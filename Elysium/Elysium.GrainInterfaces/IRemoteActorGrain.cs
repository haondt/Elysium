using DotNext;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorGrain : IGrainWithStringKey
    {
        Task IngestActivityAsync(OutgoingRemoteActivityData activity);
        Task<Result<byte[]>> GetPublicKeyAsync();
        Task<Result<Uri>> GetInboxUriAsync();
    }
}
