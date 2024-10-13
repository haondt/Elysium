namespace Elysium.Grains.Queueing
{
    public interface IQueueProvider
    {
        IQueue<T> GetQueue<T>(string name);
    }
}
