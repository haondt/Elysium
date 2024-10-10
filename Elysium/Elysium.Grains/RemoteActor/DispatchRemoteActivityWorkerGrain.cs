using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces.Constants;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.RemoteActor;
using Elysium.GrainInterfaces.Services;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Elysium.Grains.RemoteActor
{
    [ImplicitStreamSubscription(GrainConstants.DispatchRemoteActivityStream)]
    public class DispatchRemoteActivityWorkerGrain(
        IGrainFactory<LocalIri> uriGrainFactory,
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
            try
            {

                var result = await httpService.PostAsync(new HttpPostData
                {
                    JsonLdPayload = data.Payload,
                    Target = data.Target,
                    Author = uriGrainFactory.GetGrain<ILocalActorAuthorGrain>(data.Sender)
                });
                if (!result.IsSuccessful)
                    logger.LogError("dispatch failed with reason {reason}", result.Reason);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "dispatch failed with exception");
            }

        }
    }
}
