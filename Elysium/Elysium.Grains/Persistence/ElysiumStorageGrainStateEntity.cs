using Haondt.Identity.StorageKey;

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

        public required string StateName { get; set; }
        public required T Value { get; set; }
    }
}
