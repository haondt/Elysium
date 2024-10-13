using Elysium.Client.Hubs;
using Elysium.Client.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces.Generics;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IActivityPubClientService, ActivityPubClientService>();
            services.AddScoped<IElysiumService, ElysiumService>();
            services.AddSingleton<IClientActorActivityDeliveryObserverRegistry, ClientActorActivityDeliveryObserverRegistry>();
            services.AddScoped<IClientActorActivityHandlerFactory, ClientActorActivityHandlerFactory>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddElysiumClientGrainFactories();
            services.AddScoped<IClientStartupService, ClientStartupService>();
            return services;
        }

        public static IServiceCollection AddElysiumClientGrainFactories(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IStorageKeyGrainFactory<>), typeof(StorageKeyGrainFactory<>));
            services.AddSingleton<IGrainFactory<LocalIri>, LocalIriGrainFactory>();
            services.AddSingleton<IGrainFactory<RemoteIri>, RemoteIriGrainFactory>();
            return services;
        }
    }
}
