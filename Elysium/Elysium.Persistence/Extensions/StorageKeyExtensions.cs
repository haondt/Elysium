using Haondt.Identity.StorageKey;

namespace Elysium.Persistence.Extensions
{
    public static class StorageKeyExtensions
    {
        public static StorageKey WithoutFinalValue(this StorageKey storageKey)
        {
            var parts = storageKey.Parts.Select((p, i) =>
            {
                if (i == storageKey.Parts.Count - 1)
                    return new StorageKeyPart(p.Type, "");
                return p;
            }).ToList();

            return StorageKey.Create(parts);
        }

        public static StorageKey<T> WithoutFinalValue<T>(this StorageKey<T> storageKey)
        {
            return ((StorageKey)storageKey).WithoutFinalValue().As<T>();
        }
    }
}
