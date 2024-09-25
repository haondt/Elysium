using Elysium.Core.Services;
using Elysium.Persistence.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }), new ElysiumStorageKeyConverter()))
        {
        }

    }
}
