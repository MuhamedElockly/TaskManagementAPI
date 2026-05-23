using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using SharedData.Constants;
using SharedData.DTOs;
using SharedData.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Projects — manage user-owned projects (requires JWT)")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILoggingService _logger;

    public ProjectController(IProjectService projectService, ILoggingService loggingService)
    {
        _projectService = projectService;
        _logger = loggingService;
    }

    
    [HttpPost]
    [SwaggerOperation(Summary = "Create project", Description = "Creates a project assigned to the authenticated user's `OwnerId`.")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProjectDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<string>.FailResponse("Project data is required"));
            }

            if (!ModelState.IsValid)
            {
                var errorDetails = ModelState
                    .Where(ms => ms.Value!.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.FailResponse("Validation failed", errorDetails));
            }

            var response = await _projectService.CreateAsync(dto);

            if (response.Success)
            {
                _logger.LogInformation("Project created successfully");
                return Ok(response);
            }

            if (response.Message == ResponseMessages.UserNotAuthenticated)
            {
                return Unauthorized(response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while creating the project."));
        }
    }

   
    [HttpGet]
    [SwaggerOperation(Summary = "List projects", Description = "Returns all projects where `OwnerId` matches the JWT subject.")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectResultDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectResultDTO>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var response = await _projectService.GetAllAsync();

            if (response.Success)
            {
                _logger.LogInformation("Projects retrieved successfully");
                return Ok(response);
            }

            if (response.Message == ResponseMessages.UserNotAuthenticated)
            {
                return Unauthorized(response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while retrieving projects."));
        }
    }

    
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get project by id", Description = "Returns the project only if it belongs to the authenticated user.")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var response = await _projectService.GetByIdAsync(id);

            if (response.Success)
            {
                _logger.LogInformation("Project retrieved successfully");
                return Ok(response);
            }

            if (response.Message == ResponseMessages.UserNotAuthenticated)
            {
                return Unauthorized(response);
            }

            if (response.Message == ResponseMessages.ProjectNotFound)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while retrieving the project."));
        }
    }

   
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update project", Description = "Updates name and description for a project owned by the current user.")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResultDTO>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProjectDTO dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<string>.FailResponse("Project data is required"));
            }

            if (!ModelState.IsValid)
            {
                var errorDetails = ModelState
                    .Where(ms => ms.Value!.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.FailResponse("Validation failed", errorDetails));
            }

            var response = await _projectService.UpdateAsync(id, dto);

            if (response.Success)
            {
                _logger.LogInformation("Project updated successfully");
                return Ok(response);
            }

            if (response.Message == ResponseMessages.UserNotAuthenticated)
            {
                return Unauthorized(response);
            }

            if (response.Message == ResponseMessages.ProjectNotFound)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while updating the project."));
        }
    }

  
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete project", Description = "Removes a project owned by the current user.")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var response = await _projectService.DeleteAsync(id);

            if (response.Success)
            {
                _logger.LogInformation("Project deleted successfully");
                return Ok(response);
            }

            if (response.Message == ResponseMessages.UserNotAuthenticated)
            {
                return Unauthorized(response);
            }

            if (response.Message == ResponseMessages.ProjectNotFound)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while deleting the project."));
        }
    }
}
