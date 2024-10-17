using Elysium.Grains.Queueing.Redis;

namespace Elysium.Grains.Queueing
{
    public class QueueSettings
    {
        public RedisQueueSettings Redis { get; set; } = new();
        public QueueStorageType LocalActorOutgoingQueueStorageType { get; set; }
        public QueueStorageType LocalActorIncomingQueueStorageType { get; set; }
        public QueueStorageType RemoteActorOutgoingQueueStorageType { get; set; }
        public QueueStorageType RemoteActorIncomingQueueStorageType { get; set; }
    }
}
