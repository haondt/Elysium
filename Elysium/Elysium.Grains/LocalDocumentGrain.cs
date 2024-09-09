using DotNext;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Hosting.Models;
using Elysium.Persistence.Services;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class LocalDocumentGrain([PersistentState(GrainConstants.GrainDocumentStorage)] IPersistentState<DocumentState> state,
        IJsonLdService jsonLdService,
        IGrainFactory grainFactory
        ) : Grain, ILocalDocumentGrain
    {
        IInstanceActorAuthorGrain _instanceActorGrain = grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<Optional<Exception>> InitializeValueAsync(LocalUri owner, JObject value)
        {
            if (state.State.Owner != null)
                return new(new InvalidOperationException("state is already initialized"));

            state.State = new DocumentState
            {
                Owner = owner,
                Value = value,
                UpdatedOnUtc = DateTime.UtcNow
            };
            await state.WriteStateAsync();
            return new();
        }

        public Task<Result<JObject>> GetValueAsync(Uri requester)
        {
            if (state.State.Value == null)
                return Task.FromResult<Result<JObject>>(new(new InvalidOperationException("State does not yet exist")));
            return Task.FromResult<Result<JObject>>(new(state.State.Value));
        }

        public Task<bool> HasValueAsync(Uri requester)
        {
            return Task.FromResult(state.State.Value != null);
        }

        public async Task<Result<JArray>> GetExpandedValueAsync(Uri requester)
        {
            var state = await GetValueAsync(requester);
            if (!state.IsSuccessful)
                return new(state.Error);

            return await jsonLdService.ExpandAsync(_instanceActorGrain, state.Value);
        }

        public Task<Optional<Exception>> SetValueAsync(LocalUri actor, JObject value)
        {
            throw new NotImplementedException();
        }

        public Task<Optional<Exception>> UpdateValueAsync(JObject value)
        {
            throw new NotImplementedException();
        }
    }
}
