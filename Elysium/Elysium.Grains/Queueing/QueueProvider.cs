namespace Elysium.Grains.Queueing
{
    public class QueueProvider(IQueueStorageProvider storageProvider) : IQueueProvider
    {
        public IQueue<T> GetQueue<T>(string name)
        {
            return new Queue<T>(storageProvider.GetStorage<T>(name));
        }
    }
}
