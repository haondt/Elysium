using Haondt.Identity.StorageKey;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public interface IGrainFactory<TIdentity>
    {
        TGrain GetGrain<TGrain>(TIdentity identity) where TGrain : IGrain<TIdentity>;
        TIdentity GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<TIdentity>;
    }
}
