using DotNext;
using Elysium.GrainInterfaces;
using Elysium.Grains.Services;
using KristofferStrube.ActivityStreams;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [ImplicitStreamSubscription("DispatchRemoteActivityStream")]
    public class DispatchRemoteActivityWorkerGrain(IGrainFactory grainFactory, ILogger<DispatchRemoteActivityWorkerGrain> logger, IGrainHttpClient<DispatchRemoteActivityWorkerGrain> httpClient) : Grain, IDispatchRemoteActivityWorkerGrain
    {
        private readonly IDispatchRemoteActivityGrain _dispatcher = grainFactory.GetGrain<IDispatchRemoteActivityGrain>(Guid.Empty);
        private readonly Queue<DispatchRemoteActivityData> _queue = new();
        private long _id;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _id = this.GetPrimaryKeyLong();
            var streamProvider = this.GetStreamProvider("SimpleStreamProvider");
            var streamId = StreamId.Create("DispatchRemoteActivityStream", _id);
            var stream = streamProvider.GetStream<DispatchRemoteActivityData>(streamId);
            var subscription = await stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync(cancellationToken);
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
            var content = new StringContent(data.Payload, Encoding.UTF8, "application/ld+json");
            var message = new HttpRequestMessage(HttpMethod.Post, data.Target)
            {
                Content = content
            };

            foreach (var (k, v) in data.Headers)
            {
                message.Headers.Add(k, v);
            }

            var host = data.Target.Host;
            var hostIntegrityGrain = grainFactory.GetGrain<IHostIntegrityGrain>(host);
            if (!await hostIntegrityGrain.ShouldSendRequest())
            {
                logger.LogError($"The host {host} is not in good standing, refusing to send message");
                return;
            }

            try
            {
                var response = await httpClient.HttpClient.SendAsync(message);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to send message to {host}");
                await hostIntegrityGrain.VoteAgainst();
                return;
            }

            await hostIntegrityGrain.VoteFor();
        }
    }
}
