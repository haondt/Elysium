using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.RemoteActor
{
    public interface IRemoteActorWorkerGrain : IGrain<RemoteIri>
    {
        Task PublishEvent(IncomingRemoteActivityData activity);
        Task IngestActivityAsync(OutgoingRemoteActivityData activity);
    }
}
