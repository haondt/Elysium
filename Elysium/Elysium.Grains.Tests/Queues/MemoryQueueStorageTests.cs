using Elysium.Grains.Queueing.Memory;

namespace Elysium.Grains.Tests.Queues
{
    public class MemoryQueueStorageTests : AbstractQueueStorageTests
    {
        private static MemoryQueueStorageProvider _storageProvider = new();
        public MemoryQueueStorageTests() : base(() => _storageProvider)
        {
        }

    }
}
