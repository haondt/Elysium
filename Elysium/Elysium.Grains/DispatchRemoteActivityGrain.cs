using Elysium.GrainInterfaces;
using Microsoft.Extensions.Options;
using Orleans.Streams;

namespace Elysium.Domain
{
    [KeepAlive]
    public class DispatchRemoteActivityGrain(IOptions<DispatchActivityGrainSettings> options) : Grain, IDispatchRemoteActivityGrain
    {
        private readonly List<IAsyncStream<DispatchRemoteActivityData>> _streams = new(options.Value.MaxWorkers);
        private readonly List<long> _loads = new(options.Value.MaxWorkers);
        private readonly Random _random = new();

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            for (int i = 0; i < options.Value.MaxWorkers; i++)
            {
                var streamId = StreamId.Create(GrainConstants.DispatchRemoteActivityStream, i);
                _streams.Add(streamProvider.GetStream<DispatchRemoteActivityData>(streamId));
                _loads.Add(0);
            }

            return base.OnActivateAsync(cancellationToken);
        }
        public Task Send(DispatchRemoteActivityData data)
        {
            var minimumLoad = _loads.Min();
            var minimallyLoadedWorkerIds = _loads.Where(i => i == minimumLoad).ToList();
            var selectedWorkerId = minimallyLoadedWorkerIds[_random.Next(minimallyLoadedWorkerIds.Count)];

            var stream = _streams[(int)selectedWorkerId];
            _loads[(int)selectedWorkerId]++;
            return stream.OnNextAsync(data);
        }

        public Task NotifyOfWorkerCompletion(long workerId)
        {
            _loads[(int)workerId]--;
            return Task.CompletedTask;
        }
    }
}
