using Domain.Contracts;

using Domain.Entities.IdentityEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstraction;
using SharedData.DTOs;
using SharedData.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
		public async Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO)
		{
			ApplicationUser applicationUser = await _userManager.FindByEmailAsync(loginDTO.Email);

			if (applicationUser != null)
			{
				bool isValidPassword = await _userManager.CheckPasswordAsync(applicationUser, loginDTO.Password);
				if (isValidPassword)
				{
					var roles = await _userManager.GetRolesAsync(applicationUser);
					var token = GenerateToken(applicationUser, (List<string>)roles);
					return new LoginResultDTO
					{
						Token = token
					};
				}
				else
				{
					return null;
				}

			}
			else
			{
				return null;
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
				  issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(3),
				claims: claims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
				);
			var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

			return stringToken.ToString();
		}
		public async Task<RegisterResultDTO> Register(RegisterDTO registerDTO)
		{
			return await _logger.LogOperation("Create new user", async () =>
			{
				RegisterResultDTO resultDTO = new RegisterResultDTO();
				ApplicationUser existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
				if (existingUser != null)
				{
					resultDTO.IsSuccess = false;
					resultDTO.Message = "User already exist";
					return resultDTO;

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
					resultDTO.IsSuccess = false;
					resultDTO.Message = "Unable to create new user";
					_logger.LogInformation("Create new application user");
					return resultDTO;
				}
				else if (!result.Succeeded)
				{
					resultDTO.IsSuccess = false;
					var errors = string.Join(",", result.Errors.Select(e => e.Description));
					resultDTO.Message = $"Registeration Failed{errors}";
					_logger.LogError("Unable to create new app user");
					return resultDTO;

				}
				else
				{
					await _userManager.AddToRoleAsync(newUser, Roles.NormalUser.ToString());
					var roles = await _userManager.GetRolesAsync(newUser);
					var token = GenerateToken(newUser, (List<string>)roles);
					resultDTO.IsSuccess = true;
					resultDTO.Message = "User registered successfylly";
					int res = await SaveUserToCustomTableAsync(newUser);
					if (res != -1)
					{
						resultDTO.UserId = res;
					}
					_logger.LogInformation("Succssefulty creating new user");
					return resultDTO;
				}
			});

		}
		private async Task<int> SaveUserToCustomTableAsync(ApplicationUser newUser)
		{
			try
			{
				//var chatUserRepo = _unitOfWork.GetRepository<ChatUser, int>();
				//var chatUser = new ChatUser()
				//{
				//	AppUserId = newUser.Id,
				//	CreatedAt = DateTime.Now,


				//};
				//await chatUserRepo.AddAsync(chatUser);
				//var res = await _unitOfWork.SaveChangesAsync();
				//if (res != 0)
				//{
				//	return chatUser.Id;
				//}
				//else
				//{
				return -1;
				//}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return -1;
			}
		}

	}
}
