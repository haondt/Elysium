using Orleans.TestingHost;

namespace Elysium.Grains.Tests.Queues
{
    public sealed class ClusterFixture<T> : IDisposable where T : new()
    {
        public TestCluster Cluster { get; } = new TestClusterBuilder(1)
            .AddSiloBuilderConfigurator<T>()
            .Build();

        public ClusterFixture() => Cluster.Deploy();
        public void Dispose() => Cluster.StopAllSilos();
    }

}
