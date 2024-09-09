using Elysium.GrainInterfaces;
using Elysium.Hosting.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class RemoteUriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<RemoteIri>
    {
        public TGrain GetGrain<TGrain>(RemoteIri identity) where TGrain : IGrain<RemoteIri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Uri.AbsoluteUri);
        }

        public RemoteIri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<RemoteIri>
        {
            return new RemoteIri { Uri = new Uri(grain.GetPrimaryKeyString()) };
        }
    }
}
