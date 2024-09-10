using Elysium.Client.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Haondt.Identity.StorageKey;
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
            services.AddScoped<IElysiumService, ElysiumService>();
            services.AddElysiumClientGrainFactories();
            return services;
        }

        public static IServiceCollection AddElysiumClientGrainFactories(this IServiceCollection services)
        {
            services.AddSingleton<IGrainFactory<StorageKey<UserIdentity>>, StorageKeyGrainFactory<UserIdentity>>();
            //services.AddSingleton<IGrainFactory<StorageKey>, StorageKeyGrainFactory>();
            services.AddSingleton<IGrainFactory<LocalIri>, LocalUriGrainFactory>();
            services.AddSingleton<IGrainFactory<RemoteIri>, RemoteUriGrainFactory>();
            return services;
        }
    }
}
