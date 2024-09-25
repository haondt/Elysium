﻿using Elysiuim.Core.Models;
using Elysium.Core.Models;
using Haondt.Core.Extensions;
using Orleans.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var redisSettings = clusterSettings.RedisSettings ?? throw new InvalidOperationException($"{nameof(ClusterSettings.RedisSettings)} is required for clustering strategy {ClusteringStrategy.Redis}.");
                    builder.UseRedisClustering(options =>
                    {
                        options.ConfigurationOptions = new ConfigurationOptions
                        {
                            EndPoints = new EndPointCollection { redisSettings.Endpoint }
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
