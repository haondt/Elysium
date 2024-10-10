using Elysium.GrainInterfaces.Generics;
using Haondt.Identity.StorageKey;

namespace Elysium.GrainInterfaces.Services
{
    public interface IStorageKeyGrainFactory<T>
    {
        IStorageKeyGrain<T> GetGrain(StorageKey<T> identity);
        StorageKey<T> GetIdentity(IStorageKeyGrain<T> grain);
    }
}
