using Elysium.Core.Models;
using Elysium.Extensions;
using Haondt.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elysium.Core.Extensions;

namespace Elysium.Extensions
{
    public static class ClientBuilderExtensions
    {
        public static IClientBuilder ConfigureCluster(this IClientBuilder builder, IConfiguration configuration)
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
