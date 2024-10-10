using Elysium.Core.Models;
using Elysium.GrainInterfaces.RemoteActor;
using Elysium.GrainInterfaces.Services;
using Orleans.Concurrency;

namespace Elysium.Grains.RemoteActor
{
    [StatelessWorker]
    public class RemoteActorWorkerGrain : Grain, IRemoteActorWorkerGrain
    {
        private readonly RemoteIri _id;
        private readonly IRemoteActorGrain _actorGrain;
        private readonly IDispatchRemoteActivityGrain _dispatchGrain;

        public RemoteActorWorkerGrain(IGrainFactory<RemoteIri> uriGrainFactory, IGrainFactory grainFactory)
        {
            _id = uriGrainFactory.GetIdentity(this);
            _actorGrain = uriGrainFactory.GetGrain<IRemoteActorGrain>(_id);
            _dispatchGrain = grainFactory.GetGrain<IDispatchRemoteActivityGrain>(Guid.Empty);
        }

        public Task IngestActivityAsync(OutgoingRemoteActivityData activity)
        {
            var dispatchData = new DispatchRemoteActivityData
            {
                Payload = activity.Payload,
                Sender = activity.Sender,
                Target = _id
            };
            return _dispatchGrain.Send(dispatchData);
        }

        public Task PublishEvent(IncomingRemoteActivityData activity)
        {
            // this should use IActivityPubHttpService to validate the signature before ingesting
            throw new NotImplementedException();
        }

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Iri.ToString());

    }
}
