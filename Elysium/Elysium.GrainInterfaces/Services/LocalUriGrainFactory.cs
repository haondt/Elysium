using Elysium.GrainInterfaces;
using Elysium.Hosting.Models;
using Microsoft.Extensions.Options;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class LocalUriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<LocalIri>
    {
        public TGrain GetGrain<TGrain>(LocalIri identity) where TGrain : IGrain<LocalIri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Iri.AbsoluteUri);
        }

        public LocalIri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<LocalIri>
        {
            return new LocalIri { Iri = new Uri(grain.GetPrimaryKeyString()) };
        }
    }
}
