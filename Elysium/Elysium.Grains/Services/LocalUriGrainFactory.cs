using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class LocalUriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<LocalUri>
    {
        public TGrain GetGrain<TGrain>(LocalUri identity) where TGrain : IGrain<LocalUri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Uri.AbsoluteUri);
        }

        public LocalUri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<LocalUri>
        {
            return new LocalUri { Uri = new Uri(grain.GetPrimaryKeyString()) };
        }
    }
}
