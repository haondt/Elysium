using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System.Collections.Concurrent;

namespace Elysium.Domain
{

    [ImplicitStreamSubscription(GrainConstants.LocalActorOutgoingProcessingStream)]
    public class LocalActorOutgoingProcessingGrain : Grain, ILocalActorOutgoingProcessingGrain
    {
        private readonly ILocalActorAuthorGrain _authorGrain;
        private readonly IInstanceActorAuthorGrain _instanceAuthorGrain;
        private readonly IPublicCollectionGrain _publicCollectionGrain;
        private readonly LocalIri _id;
        private readonly IGrainFactory<LocalIri> _grainFactory;
        private readonly IIriService _iriService;
        private readonly IDocumentService _documentService;
        private readonly IHostingService _hostingService;
        private readonly ILogger<LocalActorOutgoingProcessingGrain> _logger;
        private readonly IActivityPubHttpService _httpService;
        private StreamSubscriptionHandle<LocalActorOutgoingProcessingData>? _subscription;

        public LocalActorOutgoingProcessingGrain(IGrainFactory<LocalIri> grainFactory,
            IGrainFactory baseGrainFactory,
            IIriService iriService,
            IDocumentService documentService,
            IHostingService hostingService,
            ILogger<LocalActorOutgoingProcessingGrain> logger,
            IActivityPubHttpService httpService)
        {
            _id = grainFactory.GetIdentity(this);
            _authorGrain = grainFactory.GetGrain<ILocalActorAuthorGrain>(_id);
            _instanceAuthorGrain = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
            _publicCollectionGrain = baseGrainFactory.GetGrain<IPublicCollectionGrain>(Guid.Empty);
            _grainFactory = grainFactory;
            _iriService = iriService;
            _documentService = documentService;
            _hostingService = hostingService;
            _logger = logger;
            _httpService = httpService;
        }
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            var streamId = StreamId.Create(GrainConstants.LocalActorOutgoingProcessingStream, _id.Iri.ToString());
            var stream = streamProvider.GetStream<LocalActorOutgoingProcessingData>(streamId);
            _subscription = await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync(cancellationToken);
        }
        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            if (_subscription != null)
                await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        // todo: figure out what to do with this comment lol
        /// <summary>
        /// Servers performing delivery to the inbox or sharedInbox properties of actors on other servers MUST
        /// provide the object property in the activity: Create, Update, Delete, Follow, Add, Remove, Like, 
        /// Block, Undo. Additionally, servers performing server to server delivery of the following activities 
        /// MUST also provide the target property: Add, Remove.
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#server-to-server-interactions"/></remarks>
        /// <param name="sender"></param>
        /// <param name="recepient"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public async Task OnNextAsync(LocalActorOutgoingProcessingData data, StreamSequenceToken? token)
        {
            var swallowExceptions = true;
            if (swallowExceptions)
                try
                {
                    await OnNextAsyncInternal(data, token);
                }
                catch
                {
                    // todo: log exception
                }
            else
                await OnNextAsyncInternal(data, token);

        }

        public async Task OnNextAsyncInternal(LocalActorOutgoingProcessingData data, StreamSequenceToken? token)
        {
            // create list of inboxes
            List<LocalIri> localRecipientIris = [];
            List<RemoteIri> remoteRecipientInboxes = [];
            List<Func<Task>> sendTasks = [];
            ConcurrentBag<(Iri Target, string Reason)> failures = [];

            // old todo: for each recepient, look them up
            // the lookup should be done by the instance actor
            // then retrieve the inbox(es)
            // if the inbox is a collection, recursively resolve it
            // but limit the recursion depth
            // also remove me from the final list of inboxes
            // this will be handled by a worker grain? or a dispatcher grain maybe... it will have both local and remote targets
            foreach (var recipient in data.Recipients)
            {
                // https://www.w3.org/TR/activitypub/#delivery
                // we need to 1) lookup the recepient (using instance actor)
                // 0) if any recepient is me, ignore that
                // 2) if the recepient is an actor (meaning it has an inbox, outbox, following and followers)
                // then deliver to the inbox property
                // 2.1) if the recepient is a collection or ordered collection
                // 3) if the collection is the public collection
                // 4) do not deliver it
                // 3.1) otherwise, recurse on the items of the collection, with a depth limit
                // 2.2) if the recepient is something else?
                // 3) idk bro throw an error
                // all the GETs should be authored by the instance grain

                if (recipient == ActivityPubConsts.PUBLIC_COLLECTION.Iri)
                {
                    await _publicCollectionGrain.IngestReferenceAsync(data.ActivityType, data.ActivityIri.Iri);
                    continue;
                }

                if (_hostingService.Host == recipient.Host)
                {
                    var localIri = new LocalIri { Iri = recipient };
                    var document = await _documentService.GetExpandedDocumentAsync(_authorGrain, localIri);
                    if (!document.IsSuccessful)
                    {
                        failures.Add((recipient, $"retriving local document {recipient} failed with reason {document.Reason}"));
                        continue;
                    }

                    if (ActivityPubJsonNavigator.IsActor(document.Value))
                    {
                        localRecipientIris.Add(localIri);
                        continue;
                    }

                    // todo: recursion
                    throw new NotImplementedException();





                }
                else
                {
                    throw new NotImplementedException(); // this is implemented wrong
                    //var remoteUri = new RemoteIri { Iri = recipient };
                    //sendTasks.Add(async () =>
                    //{
                    //    var actorState = await _httpService.GetAsync(new HttpGetData
                    //    {
                    //        Author = _instanceAuthorGrain,
                    //        Target = remoteUri
                    //    });

                    //    // todo: recursively resolve inboxes
                    //    // and pass inbox post job over to dispatch grain
                    //    throw new NotImplementedException(); 
                    //});
                }
            }

            localRecipientIris = localRecipientIris.Distinct().Where(r => r.Iri != _id.Iri).ToList();
            remoteRecipientInboxes = remoteRecipientInboxes.Distinct().ToList();

            await Task.WhenAll(localRecipientIris.Select(r =>
            {
                try
                {

                    var localActorGrain = _grainFactory.GetGrain<ILocalActorGrain>(r);
                    return localActorGrain.IngestActivityAsync(_id.Iri, data.ActivityType, data.Activity);
                }
                catch (Exception ex)
                {
                    failures.Add((r.Iri, ex.ToString()));
                    return Task.CompletedTask;
                }

            }).Concat(remoteRecipientInboxes.Select(async r =>
            {
                throw new NotImplementedException();
            })));


            // todo: log failures

        }
    }
}
