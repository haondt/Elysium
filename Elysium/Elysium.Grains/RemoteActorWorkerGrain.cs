using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Newtonsoft.Json;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [StatelessWorker]
    public class RemoteActorWorkerGrain : Grain, IRemoteActorWorkerGrain
    {
        private readonly RemoteUri _id;
        private readonly IRemoteActorGrain _actorGrain;
        private readonly IDispatchRemoteActivityGrain _dispatchGrain;

        public RemoteActorWorkerGrain(IGrainFactory<RemoteUri> uriGrainFactory, IGrainFactory grainFactory)
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

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Uri.AbsoluteUri);

    }
}
