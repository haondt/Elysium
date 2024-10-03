using Elysium.Core.Models;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorGrain : IGrain<RemoteIri>
    {
        Task<byte[]> GetPublicKeyAsync();
        //Task<JArray> GetInboxAsync();
    }
}
