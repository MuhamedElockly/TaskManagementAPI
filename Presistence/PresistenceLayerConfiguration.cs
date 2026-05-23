using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presistence.Data;
using Presistence.Repositories;
using Domain.Contracts;

namespace Presistence
{
    public static class PresistenceLayerConfiguration
    {
        public static IServiceCollection AddPresistenceConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
            ));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();


            return services;
        }
    }
}