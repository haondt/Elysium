using Elysium.GrainInterfaces.Queueing;

namespace Elysium.Grains.Queueing
{
    [KeepAlive]
    public class QueueStartupGrain(IEnumerable<QueueDescriptor> descriptors,
        IGrainFactory grainFactory) : Grain, IQueueStartupGrain
    {
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            foreach (var descriptor in descriptors)
            {
                var grainType = typeof(IQueueGrain<>).MakeGenericType(descriptor.PayloadType);
                var queueGrain = (IQueueGrain)grainFactory.GetGrain(grainType, descriptor.Name);
                await queueGrain.EnsureActivatedAsync();
            }
        }

        public Task EnsureActivatedAsync() { return Task.CompletedTask; }
    }
}
