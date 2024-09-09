using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Persistence.Services
{
    public class ElysiumSqliteStorageSettings
    {
        public string DatabasePath { get; set; } = "./ElysiumData.db";
    }
}
