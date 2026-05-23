using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Presentation.Controllers;
using System.Reflection;

namespace ECommerce.Swagger;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Task Management API",
                Version = "v1",
                Description = """
                    REST API for managing projects and tasks with JWT authentication.

                    **Getting started**
                    1. Call `POST /api/Auth/Register` or `POST /api/Auth/Login` to obtain tokens.
                    2. Click **Authorize**, enter `Bearer {your-access-token}` (include the word Bearer).
                    3. Call protected Project and Task endpoints.
                    """,
                Contact = new OpenApiContact
                {
                    Name = "Task Management API",
                },
            });

            options.EnableAnnotations();

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header. Example: `Bearer eyJhbGciOiJIUzI1NiIs...`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme,
                },
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() },
            });

            IncludeXmlComments(options, typeof(AuthController).Assembly);
            IncludeXmlComments(options, typeof(SharedData.DTOs.LoginDTO).Assembly);
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Task Management API";
            options.DisplayRequestDuration();
            options.EnablePersistAuthorization();
            options.EnableDeepLinking();
        });

        return app;
    }

    private static void IncludeXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options, Assembly assembly)
    {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    }
}
