using Elysium.Core.Models;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorGrain : IGrain<RemoteIri>
    {
        Task<byte[]> GetPublicKeyAsync();
        //Task<JArray> GetInboxAsync();
    }
}
