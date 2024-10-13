namespace Elysium.Grains.Queueing
{
    public interface IQueue<T>
    {
        Task Enqueue(T payload);
    }
}