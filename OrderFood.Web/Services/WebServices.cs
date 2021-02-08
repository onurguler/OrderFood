using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFood.Application.Services;
using OrderFood.Infrastructure.Services;

namespace OrderFood.Web.Services
{
    public static class WebServices
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();

            services.AddTransient<WebBaseManager, WebBaseManager>();

            return services;
        }
    }
}
