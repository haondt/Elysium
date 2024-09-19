using Haondt.Core.Collections;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Services
{
    public class ElysiumStorageKeyConverter : IElysiumStorageKeyConverter
    {
        private readonly StorageKeySerializerSettings _serializerSettings = new()
        {
            TypeNameStrategy = TypeNameStrategy.SimpleTypeConverter,
        };
        public string Serialize(StorageKey storageKey) => StorageKeyConvert.Serialize(storageKey, _serializerSettings);
        public StorageKey Deserialize(string data) => StorageKeyConvert.Deserialize(data, _serializerSettings);
        public StorageKey<T> Deserialize<T>(string data) => StorageKeyConvert.Deserialize<T>(data, _serializerSettings);
    }
}
