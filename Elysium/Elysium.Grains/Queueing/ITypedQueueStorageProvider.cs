namespace Elysium.Grains.Queueing
{
    public interface ITypedQueueStorageProvider : IQueueStorageProvider
    {
        QueueStorageType Type { get; }
    }
}
