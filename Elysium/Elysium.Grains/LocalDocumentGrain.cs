using DotNext;
using Elysium.GrainInterfaces;
using Elysium.Grains.Services;
using Elysium.Persistence.Services;
using JsonLD.Core;
using KristofferStrube.ActivityStreams;
using Newtonsoft.Json.Linq;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class LocalDocumentGrain([PersistentState(nameof(DocumentState))] IPersistentState<DocumentState> state,
        IJsonLdService jsonLdService
        ) : Grain, ILocalDocumentGrain
    {
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

        public Task<Result<JObject>> GetValueAsync()
        {
            if (state.State.Value == null)
                return Task.FromResult<Result<JObject>>(new(new InvalidOperationException("State does not yet exist")));
            return Task.FromResult<Result<JObject>>(new(state.State.Value));
        }

        public async Task<Result<JArray>> GetExpandedValueAsync()
        {
            var state = await GetValueAsync();
            if (!state.IsSuccessful)
                return new(state.Error);

            return await jsonLdService.ExpandAsync(state.Value);
        }
    }
}
