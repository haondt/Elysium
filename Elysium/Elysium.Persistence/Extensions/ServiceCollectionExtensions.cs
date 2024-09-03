using Elysium.Persistence.Services;
using Haondt.Core.Extensions;
using Haondt.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var persistenceSettings = configuration.GetSection<PersistenceSettings>();
            switch (persistenceSettings.Driver)
            {
                case PersistenceDrivers.Memory:
                    services.AddSingleton<IElysiumStorage, ElysiumMemoryStorage>();
                    break;
                case PersistenceDrivers.File:
                    services.AddSingleton<IElysiumStorage, ElysiumFileStorage>();
                    break;
            }

            return services;
        }
    }
}
