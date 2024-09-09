using Elysium.Persistence.Services;
using Haondt.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ElysiumPersistenceSettings>(configuration.GetSection(nameof(ElysiumPersistenceSettings)));
            var persistenceSettings = configuration.GetSection<ElysiumPersistenceSettings>();
            switch (persistenceSettings.Driver)
            {
                case ElysiumPersistenceDrivers.Memory:
                    services.AddSingleton<IElysiumStorage, ElysiumMemoryStorage>();
                    break;
                case ElysiumPersistenceDrivers.File:
                    services.AddSingleton<IElysiumStorage, ElysiumFileStorage>();
                    break;
                case ElysiumPersistenceDrivers.Sqlite:
                    services.AddSingleton<IElysiumStorage, ElysiumSqliteStorage>();
                    break;
            }

            return services;
        }
    }
}
