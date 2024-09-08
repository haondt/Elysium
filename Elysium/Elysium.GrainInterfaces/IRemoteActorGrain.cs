using DotNext;
using Elysium.Hosting.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorGrain : IGrain<RemoteUri>
    {
        Task<Result<byte[]>> GetPublicKeyAsync();
        Task<Result<Uri>> GetInboxUriAsync();
    }
}
