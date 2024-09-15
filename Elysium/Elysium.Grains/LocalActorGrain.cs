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
using Newtonsoft.Json.Linq;
using Elysium.GrainInterfaces.Services;
using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Newtonsoft.Json;
using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Orleans.Streams;
using Elysium.Hosting.Services;
using Elysium.ActivityPub.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Elysium.Cryptography.Services;
using Elysium.Persistence.Services;

namespace Elysium.Grains
{
    // for when we get to the actors url body
    // need to add the public key
    // for multibase need the right contexts
    // 
    // "https://www.w3.org/ns/activitystreams",
    //  "https://www.w3.org/ns/did/v1",
    //   "https://w3id.org/security/multikey/v1"
    // see https://web.archive.org/web/20221218063101/https://web-payments.org/vocabs/security#publicKey
    // and https://swicg.github.io/activitypub-http-signature/#how-to-obtain-a-signature-s-public-key
    // and https://www.w3.org/TR/controller-document/#dfn-publickeymultibase
    public class LocalActorGrain : Grain, ILocalActorGrain
    {
        private readonly IPersistentState<LocalActorState> _state;
        private readonly IHostingService _hostingService;
        private readonly IElysiumStorage _storage;
        private readonly IJsonLdService _jsonLdService;
        private readonly IUserCryptoService _cryptoService;
        private readonly IDocumentService _documentService;
        private readonly IGrainFactory _grainFactory;
        private readonly IGrainFactory<RemoteIri> _remoteGrainFactory;
        private readonly IIriService _iriService;
        private readonly IGrainFactory<LocalIri> _localGrainFactory;
        private readonly LocalIri _id;
        private readonly ILocalActorAuthorGrain _authorGrain;
        private readonly IAsyncStream<LocalActorOutgoingProcessingData> _outgoingStream;
        private readonly IAsyncStream<LocalActorIncomingProcessingData> _incomingStream;

        private byte[]? _signingKey;

        public LocalActorGrain(
            [PersistentState(nameof(LocalActorState), GrainConstants.GrainStorage)] IPersistentState<LocalActorState> state,
            IHostingService hostingService,
            IElysiumStorage storage,
            IJsonLdService jsonLdService,
            IUserCryptoService cryptoService,
            IDocumentService documentService,
            IIriService iriService,
            IGrainFactory grainFactory,
            IGrainFactory<RemoteIri> remoteGrainFactory,
            IGrainFactory<LocalIri> localGrainFactory)
        {
            _state = state;
            _hostingService = hostingService;
            _storage = storage;
            _jsonLdService = jsonLdService;
            _cryptoService = cryptoService;
            _documentService = documentService;
            _grainFactory = grainFactory;
            _remoteGrainFactory = remoteGrainFactory;
            _iriService = iriService;
            _localGrainFactory = localGrainFactory;
            _id = localGrainFactory.GetIdentity(this);
            _authorGrain = localGrainFactory.GetGrain<ILocalActorAuthorGrain>(_id);

            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            var outgoingStreamId = StreamId.Create(GrainConstants.LocalActorOutgoingProcessingStream, _id.Iri.ToString());
            _outgoingStream = streamProvider.GetStream<LocalActorOutgoingProcessingData>(outgoingStreamId);
            var incomingStreamId = StreamId.Create(GrainConstants.LocalActorIncomingProcessingStream, _id.Iri.ToString());
            _incomingStream = streamProvider.GetStream<LocalActorIncomingProcessingData>(incomingStreamId);
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _state.ReadStateAsync();
            if (_state.State.IsInitialized)
                _signingKey = _cryptoService.DecryptPrivateKey(_state.State.EncryptedSigningKey);
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task InitializeAsync(ActorRegistrationDetails registrationDetails)
        {
            // todo: saga pattern
            if (_state.State.IsInitialized)
                throw new InvalidOperationException($"actor {_id} is already initialized");

            var actorDocument = await GenerateDocumentAsync(registrationDetails.PublicKey, registrationDetails.EncryptedSigningKey);

            _state.State = new LocalActorState
            {
                EncryptedSigningKey = registrationDetails.EncryptedSigningKey,
                Id = _id,
                IsInitialized = true,
                PublicKey = registrationDetails.PublicKey,
                Type = registrationDetails.Type,
            };
            await _state.WriteStateAsync();

            var result = await _documentService.CreateDocumentAsync(_id, _id, actorDocument, [], []);
            if (!result.IsSuccessful)
                throw new Exception("todo: saga pattern would've saved ya here");


            _signingKey = _cryptoService.DecryptPrivateKey(registrationDetails.EncryptedSigningKey);
        }
        private async Task<JObject> GenerateDocumentAsync(string publicKey, string type)
        {
            var publicKeyPem = _cryptoService.EncodePublicKeyToPemX509(publicKey);
            var iriCollection = _iriService.GetLocalActorIris(_id);
            var localizedUsername = _iriService.GetLocalizedActornameForLocalIri(_id);

            var expandedDocument = new ActivityPubJsonBuilder()
                .Id(_id.Iri)
                .Type(type)
                .Inbox(iriCollection.Inbox.Iri)
                .Outbox(iriCollection.Outbox.Iri)
                .Followers(iriCollection.Followers.Iri)
                .Following(iriCollection.Following.Iri)
                .PreferredUsername(localizedUsername)
                .PublicKeyPem(iriCollection.PublicKey.Iri, _id.Iri, publicKeyPem)
                .Build();

            var instanceActor = _grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

            return await _jsonLdService.CompactAsync(instanceActor, expandedDocument);
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInitializedAsync()
        {
            return Task.FromResult(_state.State.IsInitialized);
        }

        //public async Task<Optional<Exception>> InitializeDocument()
        //{
        //    throw new NotImplementedException();
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
        //}

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


        public async Task IngestActivityAsync(Iri sender, JToken activity)
        {
            // todo: save the activity (id) in my inbox grain, if it has an id
            await _incomingStream.OnNextAsync(new LocalActorIncomingProcessingData
            {
                Activity = activity,
                Sender = sender
            });
        }

        // this iri is the iri of the *activity*, not the object.
        // you willl have to query the iri to get the activity object, then get the object object from that.
        // probably a good idea in case the grain or other downstream services makes changes to the object
        public async Task<(LocalIri ActivityUri, LocalIri ObjectUri)> PublishActivity(ActivityType type, JArray expandedObject)
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



            if (type == ActivityType.Create)
            {
                var mainObjectResult = expandedObject.Single().As<JObject>();

                // get recepients
                static List<Iri> ExtractListIdValues(string name, JObject parent)
                {
                    if (!parent.TryGetValue(name, out var values))
                        return new(new List<Iri>());
                    if (values is not JArray ja)
                        throw new JsonException($"{name} was not in the expected format");
                    var output = new List<Iri>();
                    foreach (var value in values)
                    {
                        if (value is not JObject jv)
                            throw new JsonException($"one or more values of {name} was not in the expected format");
                        if (jv.Count != 1)
                            throw new JsonException($"one or more values of {name} did not have exactly 1 key");
                        if (!jv.TryGetValue("@id", out var typeValueToken))
                            throw new JsonException($"one or more values of {name} did not have a @id key");
                        if (typeValueToken is not JValue typeValue || typeValue.Type != JTokenType.String)
                            throw new JsonException($"one or more values of {name} did not have a string key");
                        var typeString = typeValue.Value<string>();
                        if (string.IsNullOrEmpty(typeString))
                            throw new JsonException($"one or more values of {name} had an empty string value");

                        output.Add(Iri.FromUnencodedString(typeString));
                    }

                    return output;
                }

                var ccList = ExtractListIdValues(JsonLdTypes.CC, mainObjectResult);
                var bccList = ExtractListIdValues(JsonLdTypes.BCC, mainObjectResult);
                var toList = ExtractListIdValues(JsonLdTypes.TO, mainObjectResult);
                var btoList = ExtractListIdValues(JsonLdTypes.BTO, mainObjectResult);
                var audienceList = ExtractListIdValues(JsonLdTypes.AUDIENCE, mainObjectResult);

                var recepients = new HashSet<Iri>();
                recepients.UnionWith(ccList);
                recepients.UnionWith(bccList);
                recepients.UnionWith(toList);
                recepients.UnionWith(btoList);
                recepients.UnionWith(audienceList);




                // todo: the id generation strategy should be moved to a service
                LocalIri objectIri = await _documentService.ReserveDocumentIriAsync(_id, () =>
                {
                    var objectId = _cryptoService.GenerateDocumentId();
                    // todo: this should maybe? depend on the activityobject type, e.g. messages should be at useruri/messages/1234
                    return _iriService.GetAnonymousObjectIri(objectId);
                }, 5); // todo: put this in an appsetting

                var activityIri = await _documentService.ReserveDocumentIriAsync(_id, () =>
                {
                    var activityId = _cryptoService.GenerateDocumentId();
                    return _iriService.GetActorScopedActivityIri(_id, activityId);
                }, 5); // todo: put this in an appsetting

                var activity = Compositor.Composit(new CreateActivityDetails
                {
                    Id = activityIri.Iri,
                    Actor = _id.Iri,
                    Bcc = bccList,
                    Bto = btoList,
                    Cc = ccList,
                    To = toList,
                    Object = objectIri.Iri,
                    AttributedTo = _id.Iri,
                });


                mainObjectResult["@id"] = objectIri.ToString();
                var compactedObject = await _jsonLdService.CompactAsync(_authorGrain, expandedObject);
                var compactedActivity = await _jsonLdService.CompactAsync(_authorGrain, activity);

                // create the object
                var result = await _documentService.CreateDocumentAsync(_id, objectIri, compactedObject, btoList, bccList);
                if (!result.IsSuccessful)
                    throw new InvalidOperationException($"Failed to create document due to reason {result.Reason}");

                // create the activity
                result = await _documentService.CreateDocumentAsync(_id, activityIri, compactedActivity, btoList, bccList);
                if (!result.IsSuccessful)
                    throw new InvalidOperationException($"Failed to create document due to reason {result.Reason}");

                // TODO: the activity should also be available at a more well known url, if the type is understood. e.g. toots go at users/fred/toot/12345

                // strip bto, bcc and dereference activity
                var outgoingActivity = Compositor.Composit(new PrePublishActivityDetails
                {
                    ReferencedActivityWithBtoBcc = activity,
                    ObjectWithBtoBcc = expandedObject
                });

                var outgoingCompactedActivity = await _jsonLdService.CompactAsync(_authorGrain, outgoingActivity);

                // dispatch the activity
                // todo: we can optimize this, since when communicating local -> local there's no point in compacting the activity
                await _outgoingStream.OnNextAsync(new LocalActorOutgoingProcessingData
                {
                    Activity = outgoingCompactedActivity,
                    Recipients = recepients.ToList()
                });

                return (activityIri, objectIri);

            }
            else
            {
                throw new NotSupportedException($"ActivityType {type} not yet suported");
            }
        }


        public Task PublishTransientActivity(ActivityType type, JObject @object)
        {
            throw new NotImplementedException();
        }

        public Task<LocalActorState> GetStateAsync()
        {
            if (!_state.State.IsInitialized)
                throw new InvalidOperationException($"actor {_id} is not initialized");
            return Task.FromResult(_state.State);
        }

        public Task<byte[]> GetSigningKeyAsync()
        {
            if (!_state.State.IsInitialized)
                throw new InvalidOperationException($"actor {_id} is not initialized");
            return Task.FromResult(_signingKey!);
        }

    }
}
