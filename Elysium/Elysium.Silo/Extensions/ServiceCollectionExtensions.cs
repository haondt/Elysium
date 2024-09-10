using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains;
using Elysium.Grains.Services;
using Elysium.Hosting.Services;
using Elysium.Server.Services;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Silo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumSiloServices(this IServiceCollection services)
        {
            //services.AddSingleton<IHostingService, HostingService>();
            //services.AddScoped<IActivityPubClientService, ActivityPubClientService>();
            services.AddElysiumSiloGrainFactories();
            return services;
        }

        public static IServiceCollection AddElysiumSiloGrainFactories(this IServiceCollection services)
        {
            services.AddSingleton<IGrainFactory<StorageKey<UserIdentity>>, StorageKeyGrainFactory<UserIdentity>>();
            //services.AddSingleton<IGrainFactory<StorageKey>, StorageKeyGrainFactory>();
            services.AddSingleton<IGrainFactory<LocalIri>, LocalUriGrainFactory>();
            services.AddSingleton<IGrainFactory<RemoteIri>, RemoteUriGrainFactory>();
            services.AddSingleton<IGrainFactory<StorageKey<UserIdentity>>, StorageKeyGrainFactory<UserIdentity>>();
            return services;
        }
    }
}
