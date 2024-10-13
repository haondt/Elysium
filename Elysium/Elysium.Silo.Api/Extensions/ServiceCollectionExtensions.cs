using Elysium.Core.Models;
using Elysium.Domain.Persistence;
using Elysium.GrainInterfaces.Generics;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Elysium.Silo.Api.Services;
using Elysium.Silo.Api.SiloStartupParticipants;
using Orleans.Runtime.Hosting;

namespace Elysium.Silo.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumSiloServices(this IServiceCollection services)
        {
            //services.AddSingleton<IHostingService, HostingService>();
            //services.AddScoped<IActivityPubClientService, ActivityPubClientService>();
            services.AddElysiumSiloGrainFactories();
            services.AddSingleton<IDevHandler, DevHandler>();

            services.AddSingleton<ISiloStartupParticipant, QueueStartupParticipant>();
            return services;
        }

        public static IServiceCollection AddElysiumSiloGrainFactories(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IStorageKeyGrainFactory<>), typeof(StorageKeyGrainFactory<>));
            services.AddSingleton<IGrainFactory<LocalIri>, LocalIriGrainFactory>();
            services.AddSingleton<IGrainFactory<RemoteIri>, RemoteIriGrainFactory>();
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
