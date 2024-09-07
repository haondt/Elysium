using Elysium.Hosting.Services;
using Elysium.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumHostingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostingSettings>(configuration.GetSection(nameof(HostingSettings)));
            services.AddSingleton<IHostingService, HostingService>();
            return services;
        }
    }
}
