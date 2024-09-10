using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public interface IStorageKeyGrainFactory<T>
    {
        IStorageKeyGrain<T> GetGrain(StorageKey<T> identity);
        StorageKey<T> GetIdentity(IStorageKeyGrain<T> grain);
    }
}
