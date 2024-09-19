using Elysium.Core.Models;
using Elysium.Core.Services;
using Elysium.GrainInterfaces;
using Haondt.Identity.StorageKey;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class StorageKeyGrainFactory<T>(IGrainFactory grainFactory, IElysiumStorageKeyConverter converter) : IStorageKeyGrainFactory<T>
    {

        public IStorageKeyGrain<T> GetGrain(StorageKey<T> identity)
        {
            return grainFactory.GetGrain<IStorageKeyGrain<T>>(converter.Serialize(identity));
        }

        public StorageKey<T> GetIdentity(IStorageKeyGrain<T> grain)
        {
            return converter.Deserialize<T>(grain.GetPrimaryKeyString());
        }
    }

}
