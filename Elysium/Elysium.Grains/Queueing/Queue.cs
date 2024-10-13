namespace Elysium.Grains.Queueing
{
    public class Queue<T>(IQueueStorage<T> storage) : IQueue<T>
    {
        public Task Enqueue(T payload)
        {
            return storage.Enqueue(payload);
        }
    }
}
