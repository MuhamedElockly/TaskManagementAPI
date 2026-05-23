using Domain.Contracts;



using Domain.Entities.IdentityEntity;

using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using Service.Specefication_Implementation.TokenSpecifications;
using ServiceAbstraction;

using SharedData.DTOs;

using SharedData.Enums;

using SharedData.Wrapper;

using System;

using System.Collections.Generic;

using System.IdentityModel.Tokens.Jwt;

using System.Linq;

using System.Security.Claims;

using System.Security.Cryptography;

using System.Security.Cryptography.X509Certificates;

using System.Text;

using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Service.CoreServices

{

    public class AuthService : IAuthService

    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IConfiguration _configuration;

        private readonly ILoggingService _logger;

        public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IConfiguration configuration, ILoggingService logger)

        {

            _unitOfWork = unitOfWork;

            _userManager = userManager;

            _configuration = configuration;

            _logger = logger;

        }

        public async Task<ApiResponse<LoginResultDTO>> LoginAsync(LoginDTO loginDTO)

        {

            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (applicationUser != null)

            {

                bool isValidPassword = await _userManager.CheckPasswordAsync(applicationUser, loginDTO.Password);

                if (isValidPassword)

                {

                    var roles = await _userManager.GetRolesAsync(applicationUser);
                    var token = GenerateToken(applicationUser, (List<string>)roles);
                    applicationUser.LastLoginUtc = DateTime.UtcNow;
                    await _userManager.UpdateAsync(applicationUser);
                    string refreshTokenStr = GenerateRefreshToken();
                    RefreshToken refreshToken = new RefreshToken()
                    {

                        ApplicationUserId = applicationUser.Id,
                        Token = refreshTokenStr,
                        ExpirationDate = DateTime.UtcNow.AddDays(7),

                    };

                    var refreshRepo = _unitOfWork.GetRepository<RefreshToken, int>();
                    await refreshRepo.AddAsync(refreshToken);
                    _unitOfWork.SaveChangesAsync();

                    return new ApiResponse<LoginResultDTO>()

                    {

                        Success = true,
                        Message = "Login successful",
                        Data = new LoginResultDTO
                        {
                            AccessToken = token,
                            RefreshToken = refreshTokenStr
                        }

                    };

                }
                else

                {

                    return new ApiResponse<LoginResultDTO>()

                    {

                        Success = false,

                        Message = "Invalid Login",

                        Data = null

                    };

                }



            }

            else

            {

                return new ApiResponse<LoginResultDTO>()

                {

                    Success = false,

                    Message = "Invalid Login",

                    Data = null

                };

            }



        }

        public string GenerateToken(ApplicationUser applicationUser, List<string> roles)

        {

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, applicationUser.UserName));

            claims.Add(new Claim(ClaimTypes.NameIdentifier, applicationUser.Id));

            claims.Add(new Claim(ClaimTypes.Email, applicationUser.Email));

            foreach (var role in roles)

            {

                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var cred = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                  issuer: _configuration["JWT:Issuer"],

                audience: _configuration["JWT:Audience"],

                expires: DateTime.Now.AddHours(3),

                claims: claims,

                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)

                );

            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);



            return stringToken.ToString();

        }

        public async Task<ApiResponse<RegisterResultDTO>> Register(RegisterDTO registerDTO)

        {

            return await _logger.LogOperation("Create new user", async () =>

            {



                ApplicationUser existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);

                if (existingUser != null)

                {



                    return new ApiResponse<RegisterResultDTO>()

                    {

                        Success = false,

                        Message = "User already exist",

                        Data = null

                    };



                }

                ApplicationUser newUser = new ApplicationUser()

                {

                    UserName = registerDTO.Email,

                    Email = registerDTO.Email,

                    PhoneNumber = registerDTO.PhoneNumber,

                    EmailConfirmed = true,

                    PhoneNumberConfirmed = true,





                };

                IdentityResult result = await _userManager.CreateAsync(newUser, registerDTO.Password);

                if (result == null)

                {







                    return new ApiResponse<RegisterResultDTO>()

                    {

                        Success = false,

                        Message = "Unable to create new user",

                        Data = null

                    };

                }

                else if (!result.Succeeded)

                {

                    var errors = string.Join(",", result.Errors.Select(e => e.Description));

                    _logger.LogError("Unable to create new app user");

                    return new ApiResponse<RegisterResultDTO>()

                    {

                        Success = false,

                        Message = $"Registeration Failed{errors}",

                        Data = null

                    };



                }

                else

                {

                    await _userManager.AddToRoleAsync(newUser, registerDTO.Role.ToString());

                    var roles = await _userManager.GetRolesAsync(newUser);

                    var token = GenerateToken(newUser, (List<string>)roles);

                    var resultDTO = new RegisterResultDTO()

                    {

                        Token = token,

                    };

                    return new ApiResponse<RegisterResultDTO>()

                    {

                        Success = true,

                        Message = "User registered successfylly",

                        Data = resultDTO

                    };

                }

            });



        }

        public string GenerateRefreshToken()

        {

            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        }



        public async Task<ApiResponse<RegisterResultDTO>> RefreshAccessToken(string token)

        {

            var refreshTokentRepo = _unitOfWork.GetRepository<RefreshToken, int>();

            var refreshToken = await refreshTokentRepo.GetFirstOrDefaultAsync(new RefreshTokenSpecifications(token));

            var resultDTO = new RegisterResultDTO();

            if (refreshToken != null)

            {

                var resultApiResponse = new ApiResponse<RegisterResultDTO>();

                if (refreshToken.ExpirationDate < DateTime.UtcNow)

                {

                    resultApiResponse.Success = false;

                    resultApiResponse.Message = "Expired token";

                    _logger.LogError("Expired token");

                }

                else

                {

                    var roles = await _userManager.GetRolesAsync(refreshToken.User);

                    var accessToken = GenerateToken(refreshToken.User, (List<string>)roles);

                    resultApiResponse.Success = true;

                    resultApiResponse.Message = "Token generated successfully";

                    resultDTO.Token = accessToken;

                    resultApiResponse.Data = resultDTO;

                    _logger.LogInformation(resultApiResponse.Message);



                }

                return resultApiResponse;

            }

            else

            {



                _logger.LogError("Invalid token");

                return new ApiResponse<RegisterResultDTO>()

                {

                    Success = false,

                    Message = "Invalid Token",

                    Data = resultDTO

                };

            }



        }

    }

}