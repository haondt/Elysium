﻿using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Elysium.GrainInterfaces.Constants;
using Elysium.GrainInterfaces.RemoteActor;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Elysium.Grains.Queueing;
using Newtonsoft.Json.Linq;

namespace Elysium.Grains.RemoteActor
{
    public class RemoteActorGrain : Grain, IRemoteActorGrain
    {
        private readonly IGrainFactory<RemoteIri> _grainFactory;
        private readonly IQueue<OutgoingRemoteActivityData> _incomingQueue;
        private readonly IUserCryptoService _cryptoService;
        private readonly RemoteIri _id;
        private byte[]? _publicKey;

        public RemoteActorGrain(
            IQueueProvider queueProvider,
            IGrainFactory<RemoteIri> grainFactory,
            IUserCryptoService cryptoService)
        {
            _id = grainFactory.GetIdentity(this);
            _grainFactory = grainFactory;
            _incomingQueue = queueProvider.GetQueue<OutgoingRemoteActivityData>(GrainConstants.RemoteActorOutgoingDataQueue);
            _cryptoService = cryptoService;
        }

        private async Task<byte[]> InternalGetPublicKeyAsync()
        {
            //var actorState = await _actorStateGrain.GetExpandedValueAsync();
            //if (!actorState.IsSuccessful)
            //    return new(actorState.Error);

            //var result = new ActivityPubJsonNavigator().GetPublicKey(actorState.Value);
            //if (!result.IsSuccessful)
            //    return new(result.Error);

            //if (result.Value.PublicKeyType == PublicKeyType.Pem)
            //    return _cryptoService.DecodePublicKeyFromPemX509( result.Value.PublicKey);
            //if (result.Value.PublicKeyType == PublicKeyType.Multibase)
            //    return _cryptoService.DecodeMultibaseString( result.Value.PublicKey);
            //return new(new ActivityPubException($"Unable to decode public key {result.Value.PublicKey}"));
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetPublicKeyAsync()
        {
            //if (_publicKey != null)
            //    return _publicKey;

            //var actorState = await _actorStateGrain.GetExpandedValueAsync();
            //if (!actorState.IsSuccessful)
            //    return new(actorState.Error);

            //var result = await InternalGetPublicKeyAsync();
            //if (!result.IsSuccessful)
            //    return new(result.Error);

            //_publicKey = result.Value;
            //return _publicKey;
            throw new NotImplementedException();
        }

        public async Task<Iri> GetInboxUriAsync()
        {
            // todo: proxy this to instance grain?
            //var actorState = await _actorStateGrain.GetExpandedValueAsync();
            //if (!actorState.IsSuccessful)
            //    return new(actorState.Error);

            //return _jsonNavigator.GetInbox(actorState.Value);
            throw new NotImplementedException();
        }

        public Task PublishEvent(IncomingRemoteActivityData activity)
        {
            //_activityPubService.PublishLocalActivityAsync
            // see https://www.w3.org/TR/activitypub/#inbox-forwarding
            throw new NotImplementedException();
        }

        public Task IngestActivityAsync(LocalIri sender, JToken activity)
        {
            return _incomingQueue.EnqueueAsync(new OutgoingRemoteActivityData
            {
                Payload = activity,
                Sender = sender,
                Receiver = _id
            });
        }
    }
}
