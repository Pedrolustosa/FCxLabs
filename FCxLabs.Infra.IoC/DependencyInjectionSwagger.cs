﻿using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCxLabs.Infra.IoC
{
    /// <summary>
    /// The dependency injection swagger.
    /// </summary>
    public static class DependencyInjectionSwagger
    {
        /// <summary>
        /// Add infrastructure swagger.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>An IServiceCollection.</returns>
        public static IServiceCollection AddInfrastructureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FCxLabs.API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Pedro Lustosa",
                        Email = "pedroeternalss@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/pedrolustosaengineer/")
                    }
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = @"JWT Authorization header using Bearer.
                                    Enter 'Bearer' [space] then put in your token.
                                    Example: 'Bearer 12345abcdef'",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }
    }
}
