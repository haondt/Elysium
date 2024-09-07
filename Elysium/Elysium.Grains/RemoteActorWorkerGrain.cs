using DotNext;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
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
        private readonly Uri _id;
        private readonly Optional<byte[]> _signingKey;
        IRemoteActorGrain _actorGrain;
        private readonly IDispatchRemoteActivityGrain _dispatchGrain;

        public RemoteActorWorkerGrain(IUriGrainFactory grainFactory)
        {
            _id = grainFactory.GetIdentity(this);
            _actorGrain = grainFactory.GetGrain<IRemoteActorGrain>(grainFactory.GetIdentity(this.GetPrimaryKeyString()));
            _dispatchGrain = grainFactory.GetGrain<IDispatchRemoteActivityGrain>(Guid.Empty);
        }

        public Task IngestActivityAsync(OutgoingRemoteActivityData activity)
        {
            var payload = JsonSerializer.Serialize(activity);
            var recepientInbox = await recepientGrain.GetInboxUriAsync();
            if (!recepientInbox.IsSuccessful)
                return new(recepientInbox.Error);

            var dispatchData = new DispatchRemoteActivityData
            {
                Payload = activity.Payload,
                Headers = activity.Headers,
                Target = activity.Target
            };
            return _dispatchGrain.Send(dispatchData);
        }

        public Task PublishEvent(IncomingRemoteActivityData activity)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.AbsoluteUri);

        public Task<string> SignAsync(string stringToSign)
        {
            if (!_signingKey.HasValue)
                _signingKey = new (_actorGrain.)
        }
    }
}
