using Microsoft.Extensions.DependencyInjection;
using Service.Auto_Mapper_Profile;
using Service.CoreServices;
using ServiceAbstraction;

namespace Service
{
	public static class ServiceLayerConfigurations
	{
		public static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
		{
			services.AddSingleton<ILoggingService, LoggingService>();
			services.AddAutoMapper(typeof(ProjectMappingProfile).Assembly);
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IProjectService, ProjectService>();
			services.AddScoped<ITaskService, TaskService>();
			services.AddHttpContextAccessor();

			return services;
		}
	}
}
