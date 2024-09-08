using DotNext;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
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
using Elysium.Server.Services;
using Elysium.Hosting.Models;
using Elysium.ActivityPub.Models;

namespace Elysium.Grains
{
    public abstract class LocalActorGrain : Grain, ILocalActorGrain
    {
        private readonly IPersistentState<LocalActorState> _state;
        private readonly IActivityPubService _activityPubService;
        private readonly IHostingService _hostingService;
        private readonly IElysiumStorage _storage;
        private readonly IJsonLdService _jsonLdService;
        private readonly IUserCryptoService _cryptoService;
        private readonly ITypedActorServiceFactory _typedActorServiceFactory;
        private readonly IGrainFactory<RemoteUri> _remoteGrainFactory;
        private readonly LocalUri _id;
        private Result<UserIdentity> _userIdentity;
        private Result<byte[]> _signingKey;
        private Result<ITypedActorService> _typedActorService;

        public LocalActorGrain(
            [PersistentState(nameof(LocalActorState))] IPersistentState<LocalActorState> state,
            IActivityPubService activityPubService,
            IHostingService hostingService,
            IElysiumStorage storage,
            IJsonLdService jsonLdService,
            IUserCryptoService cryptoService,
            ITypedActorServiceFactory typedActorServiceFactory,
            IGrainFactory<RemoteUri> remoteGrainFactory,
            IGrainFactory<LocalUri> grainFactory)
        {
            _state = state;
            _activityPubService = activityPubService;
            _hostingService = hostingService;
            _storage = storage;
            _jsonLdService = jsonLdService;
            _cryptoService = cryptoService;
            _typedActorServiceFactory = typedActorServiceFactory;
            _remoteGrainFactory = remoteGrainFactory;
            _id = grainFactory.GetIdentity(this);
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _state.ReadStateAsync();
            TryLoadTypedActorService();
            await base.OnActivateAsync(cancellationToken);
        }
        public async Task<Result<byte[]>> GetSigningKeyAsync()
        {
            if (!_typedActorService.IsSuccessful)
                return new(_typedActorService.Error);
            return await _typedActorService.Value.GetSigningKeyAsync(_id);
        }

        private void TryLoadTypedActorService()
        {
            if (_state.State.ActorType == ActorType.Unknown)
                return;
            _typedActorService = _typedActorServiceFactory.Create(_state.State.ActorType);
        }

        public async Task<Optional<Exception>> SetActorTypeAsync(ActorType actorType)
        {
            if (_state.State.ActorType != ActorType.Unknown)
                return new(new InvalidOperationException("actor type is already set"));
            _state.State.ActorType = actorType;
            await _state.WriteStateAsync();
            TryLoadTypedActorService();
            return new();
        }

        public async Task<Optional<Exception>> InitializeDocument()
        {
            throw new NotImplementedException();
            //if (!_typedActorService.IsSuccessful)
            //    return new(_typedActorService.Error);

            //var documentGrain = _grainFactory.GetGrain<ILocalDocumentGrain>(_id);
            //if (await documentGrain.HasValueAsync())
            //    return new(new InvalidOperationException("document is already initialized"));

            //var document = await _typedActorService.Value.GenerateDocumentAsync();
            //if (!document.IsSuccessful)
            //    return new(document.Error);

            //await documentGrain.SetValueAsync(document.Value);
            //return new();
        }

        //public async Task<Optional<Exception>> PublishActivityAsync(Activity activity)
        //{
        //    throw new NotImplementedException();
        //    //_userIdentity.Value;
        //    //// Do not use journaled grains because activities must be deleteable / updateable
        //    //// TODO: push activity to an event stream (?)
        //    //foreach(var follower in //TODO : pull followers from an event stream
        //    //await _activityPubService.PublishActivity()
        //}

        //public Task<OrderedCollection> GetPublishedActivities(Optional<Actor> requester)
        //{
        //    // TODO: pull activities from an event stream
        //    // tood: dont forget to dereference the content to avoid spoofing https://www.w3.org/TR/activitypub/#obj
        //    throw new NotImplementedException();
        //}

        public Task<Optional<Exception>> IngestActivityAsync(JObject activity)
        {
            throw new NotImplementedException();
            // see https://www.w3.org/TR/activitypub/#inbox-forwarding
            //var activityId = _jsonNavigator.GetId(activity);
            //throw new NotImplementedException();
        }

        // this uri is the uri of the *activity*, not the object.
        // you willl have to query the uri to get the activity object, then get the object object from that.
        // probably a good idea in case the grain or other downstream services makes changes to the object
        public Task<Result<Uri>> PublishActivity(ActivityType type, JObject @object)
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

            // discovering follower inbox needs to be dereferenced in case the inbox is a collection https://www.w3.org/TR/activitypub/#delivery






            throw new NotImplementedException();
        }


        public Task<Optional<Exception>> PublishTransientActivity(ActivityType type, JObject @object)
        {
            throw new NotImplementedException();
        }
    }
}
