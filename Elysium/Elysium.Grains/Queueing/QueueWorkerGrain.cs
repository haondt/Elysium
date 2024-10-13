using Elysium.GrainInterfaces.Queueing;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Logging;

namespace Elysium.Grains.Queueing
{
    public class QueueWorkerGrain<T> : Grain, IQueueWorkerGrain<T>
    {
        private readonly ILogger<QueueWorkerGrain<T>> _logger;
        private readonly QueueWorkerIdentity _identity;
        private IQueueConsumer<T> _consumer;
        private readonly IQueueGrain<T> _queue;

        public QueueWorkerGrain(IGrainFactory<QueueWorkerIdentity> workerGrainFactory,
            IQueueConsumerProvider consumerProvider,
            IGrainFactory grainFactory,
            ILogger<QueueWorkerGrain<T>> logger)
        {
            _identity = workerGrainFactory.GetIdentity(this);
            _consumer = consumerProvider.GetConsumer<T>(_identity.Queue);
            _queue = grainFactory.GetGrain<IQueueGrain<T>>(_identity.Queue);
            _logger = logger;
        }

        public async Task WorkAsync(StorageKey<T> key, T payload)
        {
            var success = true;
            try
            {
                await _consumer.ConsumeAsync(payload);
            }
            catch (Exception ex)
            {
                success = false;
                // todo: maybe logger.begin scope? I basically just want to tag this log with the grains queue name
                _logger.LogError($"consumer failed to consume payload successfully: {ex}", ex);
            }

            await _queue.NotifyWorkCompleteAsync(_identity.Id, key, success);

        }
    }
}
