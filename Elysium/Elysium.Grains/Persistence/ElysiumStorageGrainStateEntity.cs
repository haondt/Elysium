using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain.Persistence
{
    public class ElysiumStorageGrainStateEntity<T>
    {
        private const string KEY_STRING_SEPARATOR = "__";
        public static StorageKey<ElysiumStorageGrainStateEntity<T>> GetStorageKey(string serviceId, GrainId grainId)
        {
            var keyString = $"{CosmosIdSanitizer.Sanitize(serviceId)}" +
                $"{KEY_STRING_SEPARATOR}{CosmosIdSanitizer.Sanitize(grainId.Type.ToString()!)}" +
                $"{CosmosIdSanitizer.SeparatorChar}{CosmosIdSanitizer.Sanitize(grainId.Key.ToString()!)}";


            return StorageKey<ElysiumStorageGrainStateEntity<T>>.Create($"{keyString}");
        }

        public string StateName { get; set; } = default!;
        public T Value { get; set; } = default!;
    }
}
