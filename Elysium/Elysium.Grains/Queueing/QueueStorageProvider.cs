namespace Elysium.Grains.Queueing
{
    public class QueueStorageProvider(
        IEnumerable<ITypedQueueStorageProvider> typedProviders,
        IEnumerable<QueueDescriptor> descriptors) : IQueueStorageProvider
    {
        private readonly Dictionary<string, IQueueStorageProvider> _providers = descriptors.ToDictionary(d => d.Name, d => typedProviders.First(p => p.Type == d.StorageType) as IQueueStorageProvider);

        public IQueueStorage<T> GetStorage<T>(string name)
        {
            return _providers[name].GetStorage<T>(name);
        }
    }
}
