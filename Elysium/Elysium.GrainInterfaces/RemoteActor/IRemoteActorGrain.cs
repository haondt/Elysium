using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.RemoteActor
{
    public interface IRemoteActorGrain : IGrain<RemoteIri>
    {
        Task<byte[]> GetPublicKeyAsync();
        //Task<JArray> GetInboxAsync();
    }
}
