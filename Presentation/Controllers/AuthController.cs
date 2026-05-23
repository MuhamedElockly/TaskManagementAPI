using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using SharedData.DTOs;
using SharedData.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[SwaggerTag("Authentication — register, login, and refresh JWT tokens (no authorization required)")]
public class AuthController : ControllerBase
{
    private readonly ILoggingService _logger;
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, ILoggingService loggingService)
    {
        _authService = authService;
        _logger = loggingService;
    }

    
    [HttpPost("Register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates an ASP.NET Core Identity user. Returns success or conflict if the email already exists.")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RegisterResultDTO>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO registerDTO)
    {
        try
        {
            if (registerDTO == null)
            {
                return BadRequest(ApiResponse<string>.FailResponse("Register data is required"));
            }

            if (!ModelState.IsValid)
            {
                var errorDetails = ModelState
                    .Where(ms => ms.Value!.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.FailResponse("Validation failed"));
            }

            var response = await _authService.Register(registerDTO);

            if (response.Success)
            {
                return Ok(response);
            }

            if (response.Message == "User already exist")
            {
                return Conflict(response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An internal error occurred during registration."));
        }
    }

    
    [HttpPost("RefreshToken")]
    [SwaggerOperation(
        Summary = "Refresh access token",
        Description = "Exchanges a valid refresh token for a new access token and refresh token pair.")]
    [ProducesResponseType(typeof(ApiResponse<LoginResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResultDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshAccessToken([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token is null or empty");
            return BadRequest(ApiResponse<string>.FailResponse("Token data is required"));
        }

        try
        {
            var response = await _authService.RefreshAccessToken(token);

            if (response.Success)
            {
                _logger.LogInformation("Successfully refreshed token");
                return Ok(response);
            }

            _logger.LogError(response.Message ?? "Refresh failed");
            return Unauthorized(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while refreshing the token."));
        }
    }

    
    [HttpPost("Login")]
    [SwaggerOperation(
        Summary = "Login",
        Description = "Validates email and password. On success, returns `accessToken` and `refreshToken` for use in the Authorize dialog.")]
    [ProducesResponseType(typeof(ApiResponse<LoginResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResultDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
    {
        if (loginDTO == null)
        {
            _logger.LogWarning("Login data is null or empty");
            return BadRequest(ApiResponse<string>.FailResponse("Login data is required"));
        }

        try
        {
            var response = await _authService.LoginAsync(loginDTO);

            if (response.Success)
            {
                _logger.LogInformation("Successfully logged in");
                return Ok(response);
            }

            _logger.LogInformation("Failed to login");
            return Unauthorized(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred during login"));
        }
    }
}
