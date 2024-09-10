using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Persistence.Services;
using Haondt.Core.Models;
using Haondt.Persistence.Services;
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
        private readonly IInstanceActorAuthorGrain _instanceActorGrain = grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task InitializeValueAsync(LocalIri owner, JObject value)
        {
            if (state.State.Owner != null)
                throw new InvalidOperationException("state is already initialized");

            state.State = new DocumentState
            {
                Owner = owner,
                Value = value,
                UpdatedOnUtc = DateTime.UtcNow
            };
            await state.WriteStateAsync();
        }

        public Task<Result<JObject, DocumentReason>> GetValueAsync(Iri requester)
        {
            if (state.State.Value == null)
                throw new InvalidOperationException("State does not yet exist");
            return Task.FromResult<Result<JObject, DocumentReason>>(new(state.State.Value));
        }

        public Task<bool> HasValueAsync()
        {
            return Task.FromResult(state.State.Value != null);
        }

        public async Task<Result<JArray, DocumentReason>> GetExpandedValueAsync(Iri requester)
        {
            var state = await GetValueAsync(requester);
            if (!state.IsSuccessful)
                return new(state.Reason);

            return new(await jsonLdService.ExpandAsync(_instanceActorGrain, state.Value));
        }

        public Task<Result<DocumentReason>> SetValueAsync(LocalIri actor, JObject value)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DocumentReason>> UpdateValueAsync(JObject value)
        {
            throw new NotImplementedException();
        }
    }
}
