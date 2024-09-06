using DotNext;
using Elysium.GrainInterfaces;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Services;
using JsonLD.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class RemoteDocumentGrain([PersistentState(nameof(DocumentState))] IPersistentState<DocumentState> state,
        IGrainHttpClient<RemoteDocumentGrain> httpClient, IGrainFactory grainFactory,
        IOptions<RemoteDocumentSettings> options,
        IJsonLdService jsonLdService) : Grain, IRemoteDocumentGrain
    {
        private readonly RemoteDocumentSettings _settings = options.Value;
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }
        public Task SetValueAsync(JObject value)
        {
            state.State.Value = value;
            state.State.UpdatedOnUtc = DateTime.UtcNow;
            return state.WriteStateAsync();
        }
        public async Task<Result<JArray>> GetExpandedValueAsync()
        {
            var state = await GetValueAsync();
            if (!state.IsSuccessful)
                return new(state.Error);

            return await jsonLdService.ExpandAsync(state.Value);
        }

        public async Task<Result<JObject>> GetValueAsync()
        {
            if (state.State.Value != null)
            {
                if (state.State.UpdatedOnUtc != null)
                {
                    if (DateTime.UtcNow - state.State.UpdatedOnUtc > TimeSpan.FromHours(_settings.LifetimeInHours))
                    {
                        state.State.UpdatedOnUtc = null;
                        state.State.Value = null;
                    }
                    else
                    {
                        return state.State.Value;
                    }
                }
            }

            var result = await InternalGetValueAsync();
            if (!result.IsSuccessful)
                return result;

            state.State.UpdatedOnUtc = DateTime.UtcNow;
            state.State.Value = result.Value;
            await state.WriteStateAsync();
            return state.State.Value;

        }
        private async Task<Result<JObject>> InternalGetValueAsync()
        {
            var uri = new Uri(this.GetPrimaryKeyString());
            var hostIntegrityGrain = grainFactory.GetGrain<IHostIntegrityGrain>(uri.Host);
            if (!await hostIntegrityGrain.ShouldSendRequest())
                return new(new ActivityPubException($"The host {uri.Host} is not in good standing"));

            HttpResponseMessage response;
            try
            {
                response = await httpClient.HttpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                await hostIntegrityGrain.VoteAgainst();
                return new(new ActivityPubException($"Unable to retrieve actor model from {uri}"));
            }

            await hostIntegrityGrain.VoteFor();
            JObject? model;
            try
            {
                model = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                if (model == null)
                    return new(new ActivityPubException($"Unable to deserialize actor model from {uri}"));
            }
            catch
            {
                return new(new ActivityPubException($"Unable to deserialize actor model from {uri}"));
            }

            return model;
        }

    }
}
