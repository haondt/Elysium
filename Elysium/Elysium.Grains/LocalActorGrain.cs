using DotNext;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using KristofferStrube.ActivityStreams;
using Microsoft.Extensions.Options;
using Elysium.Grains.Extensions;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elysium.Persistence.Services;
using Newtonsoft.Json.Linq;
using Elysium.Authentication.Services;
using Elysium.GrainInterfaces.Services;

namespace Elysium.Grains
{
    public class LocalActorGrain : Grain, ILocalActorGrain
    {
        private readonly IActivityPubService _activityPubService;
        private readonly IHostingService _hostingService;
        private readonly IActivityPubJsonNavigator _jsonNavigator;
        private readonly IElysiumStorage _storage;
        private readonly IUserCryptoService _cryptoService;
        private readonly IGrainFactory _grainFactory;
        private readonly string _id;
        private Result<UserIdentity> _userIdentity;
        private Result<byte[]> _signingKey;
        private ILocalDocumentGrain? _stateGrain;

        public LocalActorGrain(
            IActivityPubService activityPubService,
            IHostingService hostingService,
            IActivityPubJsonNavigator jsonNavigator,
            IElysiumStorage storage,
            IUserCryptoService cryptoService,
            IGrainFactory grainFactory)
        {
            _activityPubService = activityPubService;
            _hostingService = hostingService;
            _jsonNavigator = jsonNavigator;
            _storage = storage;
            _cryptoService = cryptoService;
            _grainFactory = grainFactory;
            _id = this.GetPrimaryKeyString();
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _stateGrain = _grainFactory.GetGrain<ILocalDocumentGrain>(_hostingService.GetUriForLocalUser(_id).ToString());
            var storageKey = UserIdentity.GetStorageKey(_id);
            _userIdentity = await _storage.Get(storageKey);
            if (!_userIdentity.IsSuccessful)
                _signingKey = new(_userIdentity.Error);
            else
            {
                var encryptedPrivateKey = _userIdentity.Value.EncryptedPrivateKey;
                _signingKey = _cryptoService.DecryptPrivateKey(encryptedPrivateKey);
            }

            await base.OnActivateAsync(cancellationToken);
        }
        public Task<Result<byte[]>> GetSigningKey()
        {
            return Task.FromResult(_signingKey);
        }

        //public async Task<Result<string>> GetActorAsync()
        //{
        //    if (_state.State.Actor == null)
        //    {
        //        var result = InitializeActor();
        //        if (!result.IsSuccessful)
        //            return result;
        //        _state.State.Actor = result.Value;
        //        await _state.WriteStateAsync();
        //    }
        //    return new(_state.State.Actor);
        //}

        private Result<Actor> InitializeActor()
        {
            if (!_userIdentity.IsSuccessful)
                return new(_userIdentity.Error);
            var username = _userIdentity.Value.Username ?? _id;
            return new Actor
            {
                Id = _hostingService.GetUriForLocalUser(username).ToString(),
                Inbox = new Link { Href = _hostingService.GetLocalUserScopedUri(username, "inbox") },
                Outbox = new Link { Href = _hostingService.GetLocalUserScopedUri(username, "outbox") },
                Followers = new Link { Href = _hostingService.GetLocalUserScopedUri(username, "followers") },
                Following = new Link { Href = _hostingService.GetLocalUserScopedUri(username, "following") },
            };

        }

        public async Task<Optional<Exception>> PublishActivityAsync(Activity activity)
        {
            _userIdentity.Value;
            // Do not use journaled grains because activities must be deleteable / updateable
            // TODO: push activity to an event stream (?)
            foreach(var follower in //TODO : pull followers from an event stream
            await _activityPubService.PublishActivity()
        }

        public Task<OrderedCollection> GetPublishedActivities(Optional<Actor> requester)
        {
            // TODO: pull activities from an event stream
            throw new NotImplementedException();
        }

        public Task<Optional<Exception>> IngestActivityAsync(JObject activity)
        {
            // see https://www.w3.org/TR/activitypub/#inbox-forwarding
            var activityId = _jsonNavigator.GetId(activity);
            throw new NotImplementedException();
        }

        public Task<Optional<Exception>> PublishActivity(ActivityType type, JObject @object)
        {
            // todo: c/r/u/d the object on disk, create the activity on disk, send the activity to followers.
            // need to think of a way to link the document back to the user for the outbox,
            // maybe it is already possible by indexing the actor or "to" fields.
            // also need to filter the outbox by publicity/permission, see https://www.w3.org/TR/activitypub/#public-addressing
            // should try and make it so local and remote documents have the same storage format if we are going to query grain states
            // remember outbox also needs to be ordered

            // posting to outbox should result in 405 not allowed https://www.w3.org/TR/activitypub/#client-to-server-interactions

            // some info here https://www.w3.org/TR/activitypub/#retrieving-objects
            // i think we can use http header validation to verify who is asking for something
            // and then whether or not they have permission to view it idk, tbd i guess





            throw new NotImplementedException();
        }
    }
}
