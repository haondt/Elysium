using DotNext;
using Elysium.Authentication.Services;
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
    public class LocalActorWorkerGrain : Grain, ILocalActorWorkerGrain
    {
        private readonly LocalUri _id;
        private Optional<byte[]> _signingKey;
        ILocalActorGrain _actorGrain;
        private readonly IDispatchRemoteActivityGrain _dispatchGrain;
        private readonly IUserCryptoService _cryptoService;
        public LocalActorWorkerGrain(IUriGrainFactory grainFactory, IUserCryptoService cryptoService)
        {
            _id = grainFactory.GetIdentity(this);
            _actorGrain = grainFactory.GetGrain<ILocalActorGrain>(grainFactory.GetIdentity(this));
            _cryptoService = cryptoService;
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

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Uri.AbsoluteUri);

        public async Task<Result<string>> SignAsync(string stringToSign)
        {
            if (!_signingKey.HasValue)
            {
                var result = await _actorGrain.GetSigningKeyAsync();
                if (!result.IsSuccessful)
                    return new(result.Error);
                _signingKey = result;
            }

            return _cryptoService.Sign(stringToSign, _signingKey.Value);
        }
    }
}
