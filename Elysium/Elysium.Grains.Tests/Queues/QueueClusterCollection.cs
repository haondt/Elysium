using Elysium.Grains.Extensions;
using Elysium.Grains.Queueing;
using Elysium.Grains.Tests.Queues.Services;
using Orleans.TestingHost;

namespace Elysium.Grains.Tests.Queues
{
    [CollectionDefinition(Name)]
    public class QueueClusterCollection : ICollectionFixture<ClusterFixture<QueueSiloConfigurator>>
    {
        public const string Name = nameof(QueueClusterCollection);
    }

    public class QueueSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder
                .AddStartupTask<QueueSiloStartupTask>();
            siloBuilder.ConfigureServices(services =>
            {
                services.AddQueues(siloBuilder.Configuration);
                services.AddQueue<Job, JobConsumer>(JobConstants.Queue1, QueueStorageType.Memory, siloBuilder.Configuration);
                services.AddQueue<Job, JobConsumer>(JobConstants.Queue2, QueueStorageType.Memory, siloBuilder.Configuration);
            });
        }
    }
}
