using Microsoft.Extensions.DependencyInjection;
using Service.CoreServices;
using ServiceAbstraction;

namespace Service
{
	public static class ServiceLayerConfigurations
	{
		public static IServiceCollection AddServiceConfiguration(this IServiceCollection services)
		{
			services.AddSingleton<ILoggingService, LoggingService>();
			services.AddScoped<IAuthService, AuthService>();
			
			return services;
		}
	}
}
