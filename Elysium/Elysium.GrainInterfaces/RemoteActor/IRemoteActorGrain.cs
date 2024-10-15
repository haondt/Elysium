using Elysium.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.RemoteActor
{
    public interface IRemoteActorGrain : IGrain<RemoteIri>
    {
        Task<byte[]> GetPublicKeyAsync();
        //Task PublishEvent(IncomingRemoteActivityData activity);
        Task IngestActivityAsync(LocalIri sender, JToken activity);
    }
}
