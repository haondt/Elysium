using Elysium.GrainInterfaces.Queueing;

namespace Elysium.Grains.Tests.Queues.Services
{
    public class QueueSiloStartupTask(IGrainFactory grainFactory) : IStartupTask
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            var startupGrain = grainFactory.GetGrain<IQueueStartupGrain>(Guid.NewGuid());
            return startupGrain.EnsureActivatedAsync();
        }
    }
}
