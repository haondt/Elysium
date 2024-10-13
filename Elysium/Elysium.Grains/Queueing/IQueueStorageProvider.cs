namespace Elysium.Grains.Queueing
{
    public interface IQueueStorageProvider
    {
        IQueueStorage<T> GetStorage<T>(string name);
    }
}
