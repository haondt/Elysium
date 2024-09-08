using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
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
