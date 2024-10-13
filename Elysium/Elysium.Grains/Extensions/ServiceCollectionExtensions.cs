using Elysium.GrainInterfaces.Constants;
using Elysium.GrainInterfaces.Queueing;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Elysium.Grains.Hosting;
using Elysium.Grains.InstanceActor;
using Elysium.Grains.LocalActor;
using Elysium.Grains.Queueing;
using Elysium.Grains.Queueing.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Grains.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumGrainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HostIntegritySettings>(configuration.GetSection(nameof(HostIntegritySettings)));
            services.Configure<InstanceActorSettings>(configuration.GetSection(nameof(InstanceActorSettings)));
            services.AddTransient<IMemoryCache, MemoryCache>();

            return services;
        }

        public static IServiceCollection AddQueues(this IServiceCollection services, IConfiguration configuration)
        {
            // GrainConstants.LocalActorOutgoingProcessingStream

            // todo: some queuesettings thing that configures the persistence type

            // core
            services.AddSingleton<IQueueProvider, QueueProvider>();
            services.AddSingleton<IQueueStorageProvider, QueueStorageProvider>();
            services.AddSingleton<IGrainFactory<QueueWorkerIdentity>, QueueWorkerGrainFactory>();
            services.AddSingleton<IQueueConsumerProvider, QueueConsumerProvider>();

            // configuration ? maybe
            services.AddQueue<LocalActorOutgoingProcessingData, LocalActorOutgoingProcessingQueueConsumer>(
                GrainConstants.LocalActorOutgoingProcessingStream, QueueStorageType.Memory, configuration);

            // maybe
            services.AddSingleton<ITypedQueueStorageProvider, MemoryQueueStorageProvider>();
            //services.AddSingleton<ITypedQueueStorageProvider, RedisQueueStorageProvider>();

            return services;
        }

        public static IServiceCollection AddQueue<TPayload, TConsumer>(
            this IServiceCollection services,
            string queueName,
            QueueStorageType storageType, IConfiguration configuration)
            where TConsumer : class, IQueueConsumer<TPayload>
        {
            services.AddSingleton<QueueDescriptor>(new QueueDescriptor<TPayload>
            {
                Name = queueName,
                StorageType = storageType,
            });
            services.AddKeyedSingleton<IQueueConsumer<TPayload>, TConsumer>(queueName);

            return services;
        }

    }
}
