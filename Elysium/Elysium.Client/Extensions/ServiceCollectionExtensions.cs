using Elysium.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumClientServices(this IServiceCollection services)
        {
            services.AddScoped<IActivityPubClientService, ActivityPubClientService>();
            return services;
        }
    }
}
