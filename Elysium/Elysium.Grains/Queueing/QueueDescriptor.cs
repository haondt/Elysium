namespace Elysium.Grains.Queueing
{
    public class QueueDescriptor
    {
        public required string Name { get; set; }
        public required QueueStorageType StorageType { get; set; }
        public required Type PayloadType { get; set; }
    }

    public class QueueDescriptor<T>
    {
        public required string Name { get; set; }
        public required QueueStorageType StorageType { get; set; }

        public static implicit operator QueueDescriptor(QueueDescriptor<T> descriptor)
        {
            return new QueueDescriptor
            {
                Name = descriptor.Name,
                StorageType = descriptor.StorageType,
                PayloadType = typeof(T)
            };
        }

    }
}
