using Microsoft.Extensions.DependencyInjection;
using OrderFood.Application.Managers;

namespace OrderFood.Application.Services
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ProductManager, ProductManager>();
            services.AddTransient<CategoryManager, CategoryManager>();

            services.AddTransient<ApplicationBaseManager, ApplicationBaseManager>();

            return services;
        }
    }
}
