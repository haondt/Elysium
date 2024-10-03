using Elysium.Core.Models;

namespace Elysium.GrainInterfaces
{
    public interface IRemoteActorWorkerGrain : IGrain<RemoteIri>
    {
        Task PublishEvent(IncomingRemoteActivityData activity);
        Task IngestActivityAsync(OutgoingRemoteActivityData activity);
    }
}
