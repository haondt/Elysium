using Haondt.Identity.StorageKey;

namespace Elysium.GrainInterfaces.Services
{
    public class StorageKeyGrainFactory<T>(IGrainFactory grainFactory) : IStorageKeyGrainFactory<T>
    {

        public IStorageKeyGrain<T> GetGrain(StorageKey<T> identity)
        {
            return grainFactory.GetGrain<IStorageKeyGrain<T>>(StorageKeyConvert.Serialize(identity));
        }

        public StorageKey<T> GetIdentity(IStorageKeyGrain<T> grain)
        {
            return StorageKeyConvert.Deserialize<T>(grain.GetPrimaryKeyString());
        }
    }

}
