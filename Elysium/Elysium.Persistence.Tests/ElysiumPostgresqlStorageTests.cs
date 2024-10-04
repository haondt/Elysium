
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Options;

namespace Elysium.Persistence.Tests
{
    public class ElysiumPostgresqlStorageTests : AbstractElysiumStorageTests
    {
        public ElysiumPostgresqlStorageTests() : base(new ElysiumPostgresqlStorage(Options.Create(new ElysiumPersistenceSettings
        {
            Driver = ElysiumPersistenceDrivers.Postgres,
            PostgresqlStorageSettings = new ElysiumPostgresqlStorageSettings
            {
                Host = "localHost",
                Database = "testing",
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
