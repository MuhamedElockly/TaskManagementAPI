
using Domain.Entities.IdentityEntity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Presistence;
using Presistence.Data;
using Serilog;
using Service;
using System.Text;
using ECommerce.CustomMiddlewares;
using ECommerce.Swagger;

namespace ECommerce
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					policy => policy.AllowAnyOrigin()
									.AllowAnyHeader()
									.AllowAnyMethod()
					);
			});
		
			#region Identity & Authentication
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
						   .AddEntityFrameworkStores<ApplicationDbContext>()
						   .AddDefaultTokenProviders();
			builder.Services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;
			});


			; builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					var config = builder.Configuration;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = config["JWT:Issuer"],
						ValidAudience = config["JWT:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
					};
				});
			#endregion
			// Add services to the container.
			builder.Services.AddPresistenceConfig(builder.Configuration);// Custom extension method to add persistence services
			builder.Services.AddServiceConfiguration();
			builder.Services.AddControllers();
			builder.Host.UseSerilog((context, configuration) =>
			configuration.ReadFrom.Configuration(context.Configuration)
			);
			builder.Services.AddSwaggerDocumentation();

			var app = builder.Build();

			await IdentityRoleSeeder.SeedAsync(app.Services);

			app.UseCors("AllowAll");

			if (app.Environment.IsDevelopment())
			{
				app.UseSwaggerDocumentation();
			}

			app.UseSerilogRequestLogging();
			#region Exception Handler Middleware Configuration
			app.UseMiddleware<CustomExceptionMiddleware>();
			#endregion
			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
