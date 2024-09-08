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
using Elysium.Core.Extensions;
using Newtonsoft.Json;
using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Orleans.Streams;

namespace Elysium.Grains
{
    public abstract class LocalActorGrain : Grain, ILocalActorGrain
    {
        private readonly IPersistentState<LocalActorState> _state;
        private readonly IHostingService _hostingService;
        private readonly IElysiumStorage _storage;
        private readonly IJsonLdService _jsonLdService;
        private readonly IUserCryptoService _cryptoService;
        private readonly IGrainFactory _grainFactory;
        private readonly IGrainFactory<RemoteUri> _remoteGrainFactory;
        private readonly IGrainFactory<LocalUri> _localGrainFactory;
        private readonly LocalUri _id;
        private readonly ILocalActorAuthorGrain _authorGrain;
        private readonly IAsyncStream<LocalActorWorkData> _workStream;

        public LocalActorGrain(
            [PersistentState(nameof(LocalActorState))] IPersistentState<LocalActorState> state,
            IHostingService hostingService,
            IElysiumStorage storage,
            IJsonLdService jsonLdService,
            IUserCryptoService cryptoService,
            IGrainFactory grainFactory,
            IGrainFactory<RemoteUri> remoteGrainFactory,
            IGrainFactory<LocalUri> localGrainFactory)
        {
            _state = state;
            _hostingService = hostingService;
            _storage = storage;
            _jsonLdService = jsonLdService;
            _cryptoService = cryptoService;
            _grainFactory = grainFactory;
            _remoteGrainFactory = remoteGrainFactory;
            _localGrainFactory = localGrainFactory;
            _id = localGrainFactory.GetIdentity(this);
            _authorGrain = localGrainFactory.GetGrain<ILocalActorAuthorGrain>(_id);

            var streamProvider = this.GetStreamProvider("SimpleStreamProvider");
            var streamId = StreamId.Create("LocalActorWorkStream", _id.Uri.AbsoluteUri);
            _workStream = streamProvider.GetStream<LocalActorWorkData>(streamId);
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
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
        public async Task<Result<(LocalUri ActivityUri, LocalUri ObjectUri)>> PublishActivity(ActivityType type, JArray expandedObject)
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
                var mainObjectResult = new Result<JToken>(expandedObject).Single().As<JObject>();
                if (!mainObjectResult.IsSuccessful)
                    return new(mainObjectResult.Error);

                // only doing this because it is a create operation!!
                var setObjectAttributedToResult = mainObjectResult.Set(JsonLdTypes.ATTRIBUTED_TO, new JArray { new JObject { { "@Id", _id.Uri.AbsoluteUri } } });
                if (setObjectAttributedToResult.HasValue)
                    return new(setObjectAttributedToResult.Value);

                // get recepients
                Result<List<Uri>> ExtractListIdValues(string name, JObject parent)
                {
                    if (!parent.TryGetValue(name, out var values))
                        return new(new List<Uri>());
                    if (values is not JArray ja)
                        return new Result<List<Uri>>(new JsonException($"{name} was not in the expected format"));
                    var output = new List<Uri>();
                    foreach (var value in values)
                    {
                        if (value is not JObject jv)
                            return new Result<List<Uri>>(new JsonException($"one or more values of {name} was not in the expected format"));
                        if (jv.Count != 1) 
                            return new Result<List<Uri>>(new JsonException($"one or more values of {name} did not have exactly 1 key"));
                        if (!jv.TryGetValue("@type", out var typeValueToken))
                            return new Result<List<Uri>>(new JsonException($"one or more values of {name} did not have a @type key"));
                        if (typeValueToken is not JValue typeValue || typeValue.Type != JTokenType.String)
                            return new Result<List<Uri>>(new JsonException($"one or more values of {name} did not have a string key"));
                        var typeString = typeValue.Value<string>();
                        if (string.IsNullOrEmpty(typeString))
                            return new Result<List<Uri>>(new JsonException($"one or more values of {name} had an empty string value"));
                        try
                        {
                            output.Add(new Uri(typeString));
                        }
                        catch (Exception ex)
                        {
                            return new(ex);
                        }
                    }

                    return output;
                }

                var ccListResult = ExtractListIdValues(JsonLdTypes.CC, mainObjectResult.Value);
                if (!ccListResult.IsSuccessful)
                    return new(ccListResult.Error);
                var bccListResult = ExtractListIdValues(JsonLdTypes.BCC, mainObjectResult.Value);
                if (!bccListResult.IsSuccessful)
                    return new(bccListResult.Error);
                var toListResult = ExtractListIdValues(JsonLdTypes.TO, mainObjectResult.Value);
                if (!toListResult.IsSuccessful)
                    return new(toListResult.Error);
                var btoListResult = ExtractListIdValues(JsonLdTypes.BTO, mainObjectResult.Value);
                if (!btoListResult.IsSuccessful)
                    return new(btoListResult.Error);
                var audienceListResult = ExtractListIdValues(JsonLdTypes.BTO, mainObjectResult.Value);
                if (!audienceListResult.IsSuccessful)
                    return new(audienceListResult.Error);

                var recepients = new HashSet<Uri>();
                recepients.UnionWith(ccListResult.Value);
                recepients.UnionWith(bccListResult.Value);
                recepients.UnionWith(toListResult.Value);
                recepients.UnionWith(btoListResult.Value);
                recepients.UnionWith(audienceListResult.Value);

                // todo: for each recepient, look them up
                // then retrieve the inbox(es)
                // if the inbox is a collection, recursively resolve it
                // but limit the recursion depth
                // also remove me from the final list of inboxes
                // https://www.w3.org/TR/activitypub/#delivery
                // this will be handled by a worker grain? or a dispatcher grain maybe... it will have both local and remote targets


                var compactedObjectResult = await _jsonLdService.CompactAsync(_authorGrain, expandedObject);
                if (!compactedObjectResult.IsSuccessful)
                    return new(compactedObjectResult.Error);

                // todo: the id generation strategy should be moved to a service
                var objectId = Guid.NewGuid();
                // todo: these url schemes should all be moved to one place
                // todo: this should probably depend on the activityobject type, e.g. messages should be at useruri/messages/1234
                var objectUri = _hostingService.GetLocalUserScopedUri(_id, $"objects/{objectId}");

                var activityResult = ActivityCompositor.Composit(new CreateActivityDetails
                {
                    Actor = _id.Uri,
                    Bcc = bccListResult.Value,
                    Bto = btoListResult.Value,
                    Cc = ccListResult.Value,
                    To = toListResult.Value,
                    Object = objectUri.Uri,
                    AttributedTo = _id.Uri,
                });


                if (!activityResult.IsSuccessful)
                    return new(activityResult.Error);

                var activityId = Guid.NewGuid();
                var activityUri = _hostingService.GetLocalUserScopedUri(_id, $"activities/{objectId}");

                var compactedActivityResult = await _jsonLdService.CompactAsync(_authorGrain, activityResult.Value);
                if (!compactedActivityResult.IsSuccessful)
                    return new(compactedActivityResult.Error);

                // create the object
                var guardianGrain = _grainFactory.GetGrain<IGuardianGrain>(Guid.Empty);
                var response = await guardianGrain.TryCreateDocumentAsync(_id, objectUri, compactedObjectResult.Value, btoListResult.Value, bccListResult.Value);
                if (!response.IsSuccessful)
                    return new(response.Error);
                else if (response.Value != GuardianReason.Success) // TODO: mapp the guardian responses to exceptions? http responses?
                    return new(new InvalidOperationException($"Guardian denied document creation with reason {response.Value}"));

                // create the activity
                response = await guardianGrain.TryCreateDocumentAsync(_id, objectUri, compactedActivityResult.Value, btoListResult.Value, bccListResult.Value);
                if (!response.IsSuccessful)
                    return new(response.Error);
                else if (response.Value != GuardianReason.Success) // TODO: mapp the guardian responses to exceptions? http responses?
                    return new(new InvalidOperationException($"Guardian denied document creation with reason {response.Value}"));



                // TODO: the activity should also be available at a more well known url, if the type is understood. e.g. toots go at users/fred/toot/12345

                // strip bto, bcc and dereference activity
                var outgoingActivityResult = ActivityCompositor.Composit(new PrePublishActivityDetails
                {
                    ReferencedActivityWithBtoBcc = activityResult.Value,
                    ObjectWithBtoBcc = expandedObject
                });
                if (!outgoingActivityResult.IsSuccessful)
                    return new(outgoingActivityResult.Error);

                var outgoingCompactedActivityResult = await _jsonLdService.CompactAsync(_authorGrain, outgoingActivityResult.Value);
                if (!outgoingCompactedActivityResult.IsSuccessful)
                    return new(outgoingCompactedActivityResult.Error);

                // dispatch the activity
                await _workStream.OnNextAsync(new LocalActorWorkData
                {
                    Acivity = outgoingCompactedActivityResult.Value,
                    Recipients = recepients.ToList()
                });

                return new((activityUri, objectUri));

            }
            else
            {
                return new(new NotSupportedException($"ActivityType {type} not yet suported"));
            }
        }


        public Task<Optional<Exception>> PublishTransientActivity(ActivityType type, JObject @object)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Uri.AbsoluteUri);

        public Task<Result<string>> SignAsync(string stringToSign)
        {
            throw new NotImplementedException();
        }
    }
}
