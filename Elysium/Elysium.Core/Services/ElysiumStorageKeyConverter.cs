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
        public string GetTypeString(StorageKey storageKey)
        {
            var parts = storageKey.Parts.Select((p, i) =>
            {
                var typeString = StorageKeyConvert.ConvertStorageKeyPartType(p.Type, _serializerSettings).Replace(":", "::");
                var valueString = p.Value.Replace(":", "::");
                var valueBytes = Encoding.UTF8.GetBytes(p.Value);
                var entry = i == storageKey.Parts.Count - 1
                    ? typeString
                    : $"{typeString}{{:}}{p.Value}";
                return entry;
            });
            return string.Join(',', parts);

        }
    }
}
