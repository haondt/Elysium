using DotNext;
using Elysium.Authentication.Services;
using Elysium.GrainInterfaces;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Services;
using Elysium.GrainInterfaces.Extensions;
using KristofferStrube.ActivityStreams;
using Microsoft.Extensions.Caching.Memory;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class RemoteActorGrain : Grain, IRemoteActorGrain
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IActivityPubService _activityPubService;
        private readonly IUserCryptoService _cryptoService;
        private readonly IActivityPubJsonNavigator _jsonNavigator;
        private readonly IRemoteDocumentGrain _actorStateGrain;
        private readonly IDispatchRemoteActivityGrain _dispatchGrain;
        private readonly Uri _id;
        private byte[]? _publicKey;

        public RemoteActorGrain(
            IGrainFactory grainFactory,
            IActivityPubService activityPubService,
            IUserCryptoService cryptoService,
            IActivityPubJsonNavigator jsonNavigator)
        {
            _id = new Uri(this.GetPrimaryKeyString());
            _grainFactory = grainFactory;
            _activityPubService = activityPubService;
            _cryptoService = cryptoService;
            _jsonNavigator = jsonNavigator;
            _actorStateGrain = grainFactory.GetGrain<IRemoteDocumentGrain>(_id.ToString());
            _dispatchGrain = grainFactory.GetGrain<IDispatchRemoteActivityGrain>(Guid.Empty);
        }

        private async Task<Result<byte[]>> InternalGetPublicKeyAsync()
        {
            var actorState = await _actorStateGrain.GetExpandedValueAsync();
            if (!actorState.IsSuccessful)
                return new(actorState.Error);

            var result = _jsonNavigator.GetPublicKey(actorState.Value);
            if (!result.IsSuccessful)
                return new(result.Error);

            if (result.Value.PublicKeyType == PublicKeyType.Pem)
                return _cryptoService.DecodePublicKeyFromPemX509( result.Value.PublicKey);
            if (result.Value.PublicKeyType == PublicKeyType.Multibase)
                return _cryptoService.DecodeMultibaseString( result.Value.PublicKey);
            return new(new ActivityPubException($"Unable to decode public key {result.Value.PublicKey}"));
        }

        public async Task<Result<byte[]>> GetPublicKeyAsync()
        {
            if (_publicKey != null)
                return _publicKey;

            var actorState = await _actorStateGrain.GetExpandedValueAsync();
            if (!actorState.IsSuccessful)
                return new(actorState.Error);

            var result = await InternalGetPublicKeyAsync();
            if (!result.IsSuccessful)
                return new(result.Error);

            _publicKey = result.Value;
            return _publicKey;
        }

        public Task IngestActivityAsync(OutgoingRemoteActivityData activity)
        {
            return _dispatchGrain.Send(activity.PrepareForDispatch());
        }

        public async Task<Result<Uri>> GetInboxUriAsync()
        {
            var actorState = await _actorStateGrain.GetExpandedValueAsync();
            if (!actorState.IsSuccessful)
                return new(actorState.Error);

            return _jsonNavigator.GetInbox(actorState.Value);
        }
    }
}
