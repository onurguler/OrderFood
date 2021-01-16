using Microsoft.Extensions.DependencyInjection;
using OrderFood.Application.Services;
using OrderFood.Infrastructure.Services;

namespace OrderFood.Web.Services
{
    public static class WebServices
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            services.AddInfrastructureServices();
            services.AddApplicationServices();

            services.AddTransient<WebBaseManager, WebBaseManager>();

            return services;
        }
    }
}
