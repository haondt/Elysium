using Elysium.GrainInterfaces.Queueing;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Logging;

namespace Elysium.Grains.Queueing
{
    [KeepAlive]
    public class QueueGrain<T> : Grain, IQueueGrain<T>
    {
        private readonly ILogger<QueueGrain<T>> _logger;
        private readonly IGrainFactory<QueueWorkerIdentity> _workerGrainFactory;
        private readonly string _identity;
        private IQueueStorage<T> _storage;
        private QueueWorkerRegistry<T> _workerRegistry = new(10); // todo: appsettings

        public QueueGrain(IQueueStorageProvider storageProvider,
            ILogger<QueueGrain<T>> logger,
        IGrainFactory<QueueWorkerIdentity> workerGrainFactory)
        {
            _logger = logger;
            _workerGrainFactory = workerGrainFactory;
            _identity = this.GetPrimaryKeyString();
            _storage = storageProvider.GetStorage<T>(_identity);
        }

        public Task EnsureActivatedAsync() { return Task.CompletedTask; }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(WatchQueue, cancellationToken);
            return Task.CompletedTask;
        }

        private async Task WatchQueue()
        {
            try
            {
                _logger.LogInformation($"Starting to watch queue {_identity}");

                while (true)
                {
                    var (key, payload) = await _storage.BlockingDequeueToStage();
                    var workerId = await _workerRegistry.GetNextAvailableWorkerAsync();
                    var worker = _workerGrainFactory.GetGrain<IQueueWorkerGrain<T>>(new QueueWorkerIdentity
                    {
                        Id = workerId,
                        Queue = _identity
                    });

                    _ = worker.WorkAsync(key, payload);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ran into exception will watching queue {_identity}: {ex}. Stopping watch...", ex);
            }
        }

        // always interleave
        public async Task NotifyWorkCompleteAsync(int workerId, StorageKey<T> key, bool success)
        {
            try
            {
                if (success)
                    await _storage.CommitDequeue(key);
            }
            finally
            {
                await _workerRegistry.NotifyWorkCompleteAsync(workerId);
            }
        }
    }

}
