using DotNext;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Hosting.Models;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [ImplicitStreamSubscription(GrainConstants.DispatchRemoteActivityStream)]
    public class DispatchRemoteActivityWorkerGrain(
        IGrainFactory<LocalUri> uriGrainFactory, 
        IGrainFactory grainFactory, 
        ILogger<DispatchRemoteActivityWorkerGrain> logger,
        IActivityPubHttpService httpService) : Grain, IDispatchRemoteActivityWorkerGrain
    {
        private readonly IDispatchRemoteActivityGrain _dispatcher = grainFactory.GetGrain<IDispatchRemoteActivityGrain>(Guid.Empty);
        private long _id;
        private StreamSubscriptionHandle<DispatchRemoteActivityData>? _subscription;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _id = this.GetPrimaryKeyLong();
            var streamProvider = this.GetStreamProvider(GrainConstants.SimpleStreamProvider);
            var streamId = StreamId.Create(GrainConstants.DispatchRemoteActivityStream, _id);
            var stream = streamProvider.GetStream<DispatchRemoteActivityData>(streamId);
            _subscription = await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync(cancellationToken);
        }

        public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
        {
            if (_subscription != null)
                await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync(reason, cancellationToken);
        }

        public async Task OnNextAsync(DispatchRemoteActivityData data, StreamSequenceToken? token)
        {
            try
            {
                await SendAsync(data);
            }
            finally
            {
                await _dispatcher.NotifyOfWorkerCompletion(_id);
            }
        }

        private async Task SendAsync(DispatchRemoteActivityData data)
        {
            var result = await httpService.PostAsync(new HttpPostData
            {
                JsonLdPayload = data.Payload,
                Target = data.Target,
                Author = uriGrainFactory.GetGrain<ILocalActorAuthorGrain>(data.Sender)
            });

            if (result.HasValue)
                logger.LogError(result.Value, "Failed to dispatch request");
        }
    }
}
