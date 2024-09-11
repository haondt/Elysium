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
    public class RemoteIriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<RemoteIri>
    {
        public TGrain GetGrain<TGrain>(RemoteIri identity) where TGrain : IGrain<RemoteIri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Iri.ToString());
        }

        public RemoteIri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<RemoteIri>
        {
            return new RemoteIri { Iri = Iri.FromUnencodedString(grain.GetPrimaryKeyString()) };
        }
    }
}
