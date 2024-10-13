namespace Elysium.Grains.Queueing
{
    public interface IQueueConsumerProvider
    {
        IQueueConsumer<T> GetConsumer<T>(string queue);
    }
}
