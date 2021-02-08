using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OG.Identity.Domain.Models;
using OrderFood.Domain.Identity.Models;
using OrderFood.Infrastructure.UnitOfWorks;
using System;

namespace OrderFood.Infrastructure.Services
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DBContext>(options => options.UseSqlServer(configuration.GetConnectionString("OrderFoodContext")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Idendity config
            services.AddIdentity<OGUser, IdentityRole>().AddEntityFrameworkStores<DBContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            return services;
        }
    }
}
