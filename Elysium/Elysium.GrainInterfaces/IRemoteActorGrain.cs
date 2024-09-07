using DotNext;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorGrain : IGrainWithRemoteUriKey
    {
        Task<Result<byte[]>> GetPublicKeyAsync();
        Task<Result<Uri>> GetInboxUriAsync();
    }
}
