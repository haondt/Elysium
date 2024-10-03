using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Options;

namespace Elysium.Persistence.Tests
{
    public class ElysiumSqliteStorageTests : AbstractElysiumStorageTests
    {
        public ElysiumSqliteStorageTests() : base(new ElysiumSqliteStorage(Options.Create(new ElysiumPersistenceSettings
        {
            Driver = ElysiumPersistenceDrivers.Sqlite,
            SqliteStorageSettings = new ElysiumSqliteStorageSettings
            {
                DatabasePath = "./testing.db",
                StoreKeyStrings = true
            }
        })))
        {
            StorageKeyConvert.DefaultSerializerSettings = new StorageKeySerializerSettings
            {
                TypeNameStrategy = TypeNameStrategy.SimpleTypeConverter,
                KeyEncodingStrategy = KeyEncodingStrategy.String
            };
        }

    }
}
