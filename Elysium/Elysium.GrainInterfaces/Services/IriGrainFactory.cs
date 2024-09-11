using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class IriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<Iri>
    {
        public TGrain GetGrain<TGrain>(Iri identity) where TGrain : IGrain<Iri>
        {
            return grainFactory.GetGrain<TGrain>(identity.ToString());
        }

        public Iri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<Iri>
        {
            return Iri.FromUnencodedString(grain.GetPrimaryKeyString());
        }
    }
}
