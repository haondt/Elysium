using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains;
using Elysium.Grains.Persistence;
using Elysium.Grains.Services;
using Elysium.Hosting.Services;
using Elysium.Server.Services;
using Elysium.Silo.Api.Services;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime.Hosting;
using Orleans.Storage;
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
            services.AddSingleton<IDevHandler, DevHandler>();
            return services;
        }

        public static IServiceCollection AddElysiumSiloGrainFactories(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IStorageKeyGrainFactory<>), typeof(StorageKeyGrainFactory<>));
            services.AddSingleton<IGrainFactory<LocalIri>, LocalIriGrainFactory>();
            services.AddSingleton<IGrainFactory<RemoteIri>, RemoteIriGrainFactory>();
            services.AddSingleton<IGrainFactory<Iri>, IriGrainFactory>();
            return services;
        }

        // see https://learn.microsoft.com/en-us/dotnet/orleans/tutorials-and-samples/custom-grain-storage?pivots=orleans-7-0
        public static IServiceCollection AddElysiumStorageGrainStorage(this IServiceCollection services, string providerName)
        {
            //services.AddTransient<
            //IPostConfigureOptions<FileGrainStorageOptions>,
            //DefaultStorageProviderSerializerOptionsConfigurator<FileGrainStorageOptions>>();
            //services.AddSingletonNamedService(providerName, (p, n) =>
            //    (ILifecycleParticipant<ISiloLifecycle>)p.GetRequiredServiceByName<IGrainStorage>(n));
            services.AddGrainStorage(providerName, (sp, name) =>
                ActivatorUtilities.CreateInstance<ElysiumStorageGrainStorage>(sp));
            return services;
        }
    }
}
