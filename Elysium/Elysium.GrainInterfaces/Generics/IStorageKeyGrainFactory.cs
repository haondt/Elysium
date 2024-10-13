using Haondt.Identity.StorageKey;

namespace Elysium.GrainInterfaces.Generics
{
    public interface IStorageKeyGrainFactory<T>
    {
        IStorageKeyGrain<T> GetGrain(StorageKey<T> identity);
        StorageKey<T> GetIdentity(IStorageKeyGrain<T> grain);
    }
}
