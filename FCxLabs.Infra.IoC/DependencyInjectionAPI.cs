using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FCxLabs.Domain.Account;
using FCxLabs.Domain.Entities;
using FCxLabs.Domain.Interfaces;
using FCxLabs.Infra.Data.Service;
using FCxLabs.Infra.Data.Context;
using FCxLabs.Infra.Data.Identity;
using FCxLabs.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using FCxLabs.Infra.Data.Repository;
using FCxLabs.Application.Interfaces;

namespace FCxLabs.Infra.IoC
{
    /// <summary>
    /// The dependency injection API.
    /// </summary>
    public static class DependencyInjectionAPI
    {
        /// <summary>
        /// Add infrastructure API.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>An IServiceCollection.</returns>
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                                                                 .AddDefaultTokenProviders();

            //Mappings
            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            //Repositories
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            //Serices
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            return services;
        }
    }
}
