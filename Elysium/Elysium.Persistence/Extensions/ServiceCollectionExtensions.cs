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
            services.Configure<ElysiumPersistenceSettings>(configuration.GetSection(nameof(ElysiumPersistenceSettings)));
            var persistenceSettings = configuration.GetSection<ElysiumPersistenceSettings>();
            switch (persistenceSettings.Driver)
            {
                case ElysiumPersistenceDrivers.Memory:
                    services.AddSingleton<IElysiumStorage, ElysiumMemoryStorage>();
                    services.AddSingleton<IStorage, ElysiumMemoryStorage>();
                    break;
                case ElysiumPersistenceDrivers.Sqlite:
                    services.AddSingleton<IElysiumStorage, ElysiumSqliteStorage>();
                    services.AddSingleton<IStorage, ElysiumSqliteStorage>();
                    break;
            }

            return services;
        }
    }
}
