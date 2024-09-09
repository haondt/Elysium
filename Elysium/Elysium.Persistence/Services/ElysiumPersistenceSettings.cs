using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Persistence.Services
{
    public class ElysiumPersistenceSettings
    {
        public ElysiumPersistenceDrivers Driver { get; set; } = ElysiumPersistenceDrivers.Memory;
        public ElysiumSqliteStorageSettings SqliteStorageSettings { get; set; } = new();
    }
}
