using Haondt.Identity.StorageKey;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct StorageKeySurrogate<T>
    {
        [Id(0)]
        public string Value;
    }

    [RegisterConverter]
    public sealed class StorageKeyConverter<T> : IConverter<StorageKey<T>, StorageKeySurrogate<T>>
    {
        public StorageKey<T> ConvertFromSurrogate(in StorageKeySurrogate<T> surrogate)
        {
            return StorageKeyConvert.Deserialize<T>(surrogate.Value);
        }

        public StorageKeySurrogate<T> ConvertToSurrogate(in StorageKey<T> value)
        {
            return new StorageKeySurrogate<T> { Value = StorageKeyConvert.Serialize(value) };
        }
    }
}
