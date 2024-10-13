namespace Elysium.Grains.Queueing
{
    public interface IQueueConsumer<T>
    {
        Task ConsumeAsync(T payload);
    }
}
