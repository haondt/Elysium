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
    public class RemoteUriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<RemoteUri>
    {
        public TGrain GetGrain<TGrain>(RemoteUri identity) where TGrain : IGrain<RemoteUri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Uri.AbsoluteUri);
        }

        public RemoteUri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<RemoteUri>
        {
            return new RemoteUri { Uri = new Uri(grain.GetPrimaryKeyString()) };
        }
    }
}
