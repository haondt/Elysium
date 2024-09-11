using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Microsoft.Extensions.Options;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class LocalIriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<LocalIri>
    {
        public TGrain GetGrain<TGrain>(LocalIri identity) where TGrain : IGrain<LocalIri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Iri.ToString());
        }

        public LocalIri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<LocalIri>
        {
            return new LocalIri { Iri =  Iri.FromUnencodedString(grain.GetPrimaryKeyString()) };
        }
    }
}
