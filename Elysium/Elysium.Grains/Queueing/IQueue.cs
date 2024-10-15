namespace Elysium.Grains.Queueing
{
    public interface IQueue<T>
    {
        Task EnqueueAsync(T payload);
    }
}