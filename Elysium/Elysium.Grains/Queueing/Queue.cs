namespace Elysium.Grains.Queueing
{
    public class Queue<T>(IQueueStorage<T> storage) : IQueue<T>
    {
        public Task EnqueueAsync(T payload)
        {
            return storage.Enqueue(payload);
        }
    }
}
