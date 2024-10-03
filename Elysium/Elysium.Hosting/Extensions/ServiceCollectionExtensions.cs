using Elysium.Hosting.Services;
using Elysium.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumHostingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostingSettings>(configuration.GetSection(nameof(HostingSettings)));
            services.AddSingleton<IHostingService, HostingService>();
            services.AddSingleton<IIriService, IriService>();
            return services;
        }
    }
}
