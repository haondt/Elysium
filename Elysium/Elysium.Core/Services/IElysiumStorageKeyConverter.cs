using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Services
{
    public interface IElysiumStorageKeyConverter
    {
        StorageKey Deserialize(string data);
        StorageKey<T> Deserialize<T>(string data);
        string Serialize(StorageKey storageKey);
        string GetTypeString(StorageKey storageKey);
    }
}
