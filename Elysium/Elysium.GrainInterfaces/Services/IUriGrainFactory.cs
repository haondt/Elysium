using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public interface IUriGrainFactory
    {
        T GetGrain<T>(LocalUri uri) where T : IGrainWithLocalUriKey;
        T GetGrain<T>(RemoteUri uri) where T : IGrainWithLocalUriKey;
        LocalUri GetIdentity(IGrainWithLocalUriKey grain);
        RemoteUri GetIdentity(IGrainWithRemoteUriKey grain);
    }
}
