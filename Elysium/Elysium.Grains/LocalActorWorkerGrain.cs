using DotNext;
using Elysium.Authentication.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
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
        private readonly LocalUri _id;
        private readonly IGrainFactory<LocalUri> _grainFactory;
        private readonly IHostingService _hostingService;
        private readonly ILogger<LocalActorWorkerGrain> _logger;
        private readonly ILocalActorRegistryGrain _registryGrain;
        private readonly IActivityPubHttpService _httpService;
        private StreamSubscriptionHandle<LocalActorWorkData>? _subscription;

        public LocalActorWorkerGrain(IGrainFactory<LocalUri> grainFactory,
            IGrainFactory baseGrainFactory,
            IHostingService hostingService,
            ILogger<LocalActorWorkerGrain> logger,
            IActivityPubHttpService httpService)
        {
            _id = grainFactory.GetIdentity(this);
            _authorGrain = grainFactory.GetGrain<ILocalActorAuthorGrain>(_id);
            _instanceAuthorGrain = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
            _grainFactory = grainFactory;
            _hostingService = hostingService;
            _logger = logger;
            _registryGrain = baseGrainFactory.GetGrain<ILocalActorRegistryGrain>(Guid.Empty);
            _httpService = httpService;
        }
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            var streamId = StreamId.Create(GrainConstants.LocalActorWorkStream, _id.Uri.AbsoluteUri);
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

        // tood: figure out what to do with this comment lol
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
            // create list of inboxes
            List<LocalUri> localRecipients = [];
            List<RemoteUri> remoteRecipients = [];
            List<Func<Task>> sendTasks = [];
            foreach(var recipient in data.Recipients)
            {
                if (_hostingService.IsLocalHost(recipient))
                {
                    var localUri = new LocalUri { Uri = recipient };
                    var localActorExistsResult = await _registryGrain.ExistsActor(localUri);
                    if (!localActorExistsResult.IsSuccessful)
                    {
                        _logger.LogError(localActorExistsResult.Error, $"Failed to look up local actor {recipient.AbsoluteUri}");
                        continue;
                    }

                    var localActorGrain = _grainFactory.GetGrain<ILocalActorGrain>(localUri);
                    sendTasks.Add(() => localActorGrain.IngestActivityAsync(data.Acivity));
                }
                else
                {
                    var remoteUri = new RemoteUri { Uri = recipient };
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
