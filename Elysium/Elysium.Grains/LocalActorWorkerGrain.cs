using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Hosting.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Concurrency;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{

    [ImplicitStreamSubscription(GrainConstants.LocalActorWorkStream)]
    public class LocalActorWorkerGrain : Grain, ILocalActorWorkerGrain
    {
        private readonly ILocalActorAuthorGrain _authorGrain;
        private readonly IInstanceActorAuthorGrain _instanceAuthorGrain;
        private readonly LocalIri _id;
        private readonly IGrainFactory<LocalIri> _grainFactory;
        private readonly IDocumentService _documentService;
        private readonly IHostingService _hostingService;
        private readonly ILogger<LocalActorWorkerGrain> _logger;
        private readonly ILocalActorRegistrar _registryGrain;
        private readonly IActivityPubHttpService _httpService;
        private StreamSubscriptionHandle<LocalActorWorkData>? _subscription;

        public LocalActorWorkerGrain(IGrainFactory<LocalIri> grainFactory,
            IGrainFactory baseGrainFactory,
            IDocumentService documentService,
            IHostingService hostingService,
            ILogger<LocalActorWorkerGrain> logger,
            IActivityPubHttpService httpService)
        {
            _id = grainFactory.GetIdentity(this);
            _authorGrain = grainFactory.GetGrain<ILocalActorAuthorGrain>(_id);
            _instanceAuthorGrain = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
            _grainFactory = grainFactory;
            _documentService = documentService;
            _hostingService = hostingService;
            _logger = logger;
            _registryGrain = baseGrainFactory.GetGrain<ILocalActorRegistrar>(Guid.Empty);
            _httpService = httpService;
        }
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            var streamId = StreamId.Create(GrainConstants.LocalActorWorkStream, _id.Iri.ToString());
            var stream = streamProvider.GetStream<LocalActorWorkData>(streamId);
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
        public async Task OnNextAsync(LocalActorWorkData data, StreamSequenceToken? token)
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

        public async Task OnNextAsyncInternal(LocalActorWorkData data, StreamSequenceToken? token)
        { 
            // create list of inboxes
            List<LocalIri> localRecipients = [];
            List<RemoteIri> remoteRecipients = [];
            List<Func<Task>> sendTasks = [];

                // todo: for each recepient, look them up
                // the lookup should be done by the instance actor
                // then retrieve the inbox(es)
                // if the inbox is a collection, recursively resolve it
                // but limit the recursion depth
                // also remove me from the final list of inboxes
                // https://www.w3.org/TR/activitypub/#delivery
                // this will be handled by a worker grain? or a dispatcher grain maybe... it will have both local and remote targets
            foreach(var recipient in data.Recipients)
            {
                // this is implemented wrong
                // we need to 1) lookup the recepient
                // 2) if the recepient is an actor,
                // 3) send to the actors inbox
                // 4) if the recepient is a collection
                // 5) recurse on each item in the collection
                // all the GETs should be authored by the instance grain
                if (_hostingService.Host == recipient.Host)
                {
                    var localUri = new LocalIri { Iri = recipient };
                    var document = await _documentService.GetDocumentAsync(_authorGrain, localUri);
                    if(!await _registryGrain.HasRegisteredActor(localUri))
                        throw new ArgumentException($"No actor registered with local iri {recipient}");

                    var localActorGrain = _grainFactory.GetGrain<ILocalActorGrain>(localUri);
                    sendTasks.Add(() => localActorGrain.IngestActivityAsync(data.Activity));
                }
                else
                {
                    throw new NotImplementedException(); // this is implemented wrong
                    var remoteUri = new RemoteIri { Iri = recipient };
                    sendTasks.Add(async () =>
                    {
                        var actorState = await _httpService.GetAsync(new HttpGetData
                        {
                            Author = _instanceAuthorGrain,
                            Target = remoteUri
                        });

                        // todo: recursively resolve inboxes
                        // and pass inbox post job over to dispatch grain
                        throw new NotImplementedException(); 
                    });
                }
            }

            await Task.WhenAll(sendTasks.Select(t => t()));
        }
    }
}
