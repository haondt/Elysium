using Elysium.Core.Models;
using Elysium.Persistence.Services;
using Haondt.Core.Extensions;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.MongoDb.Extensions;
using Haondt.Persistence.MongoDb.Services;
using Haondt.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

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
                case ElysiumPersistenceDrivers.Sqlite:
                    services.AddSingleton<IElysiumStorage, ElysiumSqliteStorage>();
                    break;
                case ElysiumPersistenceDrivers.Postgres:
                    services.AddSingleton<IElysiumStorage, ElysiumPostgresqlStorage>();
                    break;
                case ElysiumPersistenceDrivers.MongoDb:
                    var mongoDbSettings = persistenceSettings.MongoDbStorageSettings ?? throw new ArgumentNullException(nameof(ElysiumPersistenceSettings.MongoDbStorageSettings));
                    services.AddMongoDb(new MongoDbSettings
                    {
                        ConnectionString = mongoDbSettings.ConnectionString
                    });
                    services.AddSingleton<IElysiumStorage, ElysiumMongoDbStorage>();
                    break;
            }
            services.AddSingleton<IStorage>(sp => sp.GetRequiredService<IElysiumStorage>());

            services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));

            return services;
        }

    }
}
