using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class StorageKeyGrainFactory<T>(IGrainFactory grainFactory) : IGrainFactory<StorageKey<T>>
    {
        public TGrain GetGrain<TGrain>(StorageKey<T> identity) where TGrain : IGrain<StorageKey<T>>
        {
            return grainFactory.GetGrain<TGrain>(StorageKeyConvert.Serialize(identity));
        }

        public StorageKey<T> GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<StorageKey<T>>
        {
            return StorageKeyConvert.Deserialize<T>(grain.GetPrimaryKeyString());
        }
    }
}
