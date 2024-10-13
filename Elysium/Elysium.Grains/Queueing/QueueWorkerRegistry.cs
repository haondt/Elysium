using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Elysium.Grains.Queueing
{
    public class QueueWorkerRegistry<T>
    {
        private readonly Channel<bool> _workerChannel;
        private readonly ConcurrentStack<int> _availableWorkers;

        public QueueWorkerRegistry(int maxWorkers)
        {
            _workerChannel = Channel.CreateBounded<bool>(new BoundedChannelOptions(maxWorkers)
            {
                SingleReader = true,
            });
            _availableWorkers = new ConcurrentStack<int>();

            for (int i = 0; i < maxWorkers; i++)
            {
                if (!_workerChannel.Writer.TryWrite(true))
                    throw new InvalidOperationException("Failed to initialize QueueWorkerRegistry");
                _availableWorkers.Push(i);
            }
        }

        public Task NotifyWorkCompleteAsync(int workerId)
        {
            _availableWorkers.Push(workerId);
            return _workerChannel.Writer.WriteAsync(true).AsTask();
        }

        public async Task<int> GetNextAvailableWorkerAsync()
        {
            _ = await _workerChannel.Reader.ReadAsync();
            var result = _availableWorkers.TryPop(out var workerId);
            if (!result)
                throw new InvalidOperationException("Received signal for available worker but no workers were on the stack");
            return workerId;
        }
    }
}
