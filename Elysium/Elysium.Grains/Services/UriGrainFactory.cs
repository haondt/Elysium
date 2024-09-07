using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class UriGrainFactory(IGrainFactory grainFactory) : IUriGrainFactory
    {
        public T GetGrain<T>(LocalUri uri) where T : IGrainWithLocalUriKey
        {
            return grainFactory.GetGrain<T>(uri.Uri.AbsoluteUri);
        }

        public T GetGrain<T>(RemoteUri uri) where T : IGrainWithLocalUriKey
        {
            return grainFactory.GetGrain<T>(uri.Uri.AbsoluteUri);
        }

        public LocalUri GetIdentity(IGrainWithLocalUriKey grain)
        {
            return new LocalUri(default) { Uri = new Uri(grain.GetPrimaryKeyString()) };
        }

        public RemoteUri GetIdentity(IGrainWithRemoteUriKey grain)
        {
            return new RemoteUri(default) { Uri = new Uri(grain.GetPrimaryKeyString()) };
        }

    }
}
