using Elysium.Persistence.Services;
namespace Elysium.Persistence.Tests
{
    public class ElysiumMemoryStorageTests : AbstractElysiumStorageTests
    {
        public ElysiumMemoryStorageTests() : base(new ElysiumMemoryStorage())
        {
        }

    }
}