using Elysium.Core.Models;
using Haondt.Core.Extensions;
using Orleans.Configuration;
using StackExchange.Redis;

namespace Elysium.Silo.Extensions
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder AddElysiumStorageGrainStorage(this ISiloBuilder builder, string providerName)
        {
            builder.Services.AddElysiumStorageGrainStorage(providerName);
            return builder;
        }
        public static ISiloBuilder ConfigureCluster(this ISiloBuilder builder, IConfiguration configuration)
        {
            var clusterSettings = configuration.GetSection<ClusterSettings>();
            switch (clusterSettings.ClusteringStrategy)
            {
                case ClusteringStrategy.Localhost:
                    builder.UseLocalhostClustering();
                    break;
                case ClusteringStrategy.Redis:
                    var redisSettings = configuration.GetRequiredSection<RedisSettings>();
                    builder.UseRedisClustering(options =>
                    {
                        options.ConfigurationOptions = new ConfigurationOptions
                        {
                            EndPoints = new EndPointCollection { redisSettings.Endpoint },
                            DefaultDatabase = clusterSettings.RedisDatabase
                        };
                    });
                    break;
                default:
                    throw new ArgumentException($"Unknown clustering strategy {clusterSettings.ClusteringStrategy}");
            }


            builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = clusterSettings.ClusterId;
                options.ServiceId = clusterSettings.ServiceId;
            });

            return builder;
        }
    }
}
