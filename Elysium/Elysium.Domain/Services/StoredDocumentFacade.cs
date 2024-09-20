using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Haondt.Core.Models;
using Haondt.Persistence.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain.Services
{
    public class StoredDocumentFacade(
        IStorageKeyGrainFactory<DocumentState> documentGrainFactory,
        IJsonLdService jsonLdService,
        IGrainFactory grainFactory,
        IGrainFactory<Iri> cacheGrainFactory) : IStoredDocumentFacade
    {
        IInstanceActorAuthorGrain _instanceAuthorGrain = grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

        private (
            IStorageKeyGrain<DocumentState> DocumentGrain,
            ICompactedDocumentCacheGrain CompactedCacheGrain,
            IExpandedDocumentCacheGrain ExpandedCacheGrain) GetGrains(Iri iri)
        {
            return (
                documentGrainFactory.GetGrain(DocumentState.GetStorageKey(iri)),
                cacheGrainFactory.GetGrain<ICompactedDocumentCacheGrain>(iri),
                cacheGrainFactory.GetGrain<IExpandedDocumentCacheGrain>(iri));
        }

        public async Task ClearAsync(Iri iri)
        {
            var grains = GetGrains(iri);
            await grains.CompactedCacheGrain.ClearAsync();
            await grains.ExpandedCacheGrain.ClearAsync();
            await grains.DocumentGrain.ClearAsync();
        }

        public Task<bool> ExistsAsync(Iri iri)
        {
            var grains = GetGrains(iri);
            return grains.DocumentGrain.ExistsAsync();
        }

        public Task<Result<DocumentState, StorageResultReason>> GetAsync(Iri iri)
        {
            var grains = GetGrains(iri);
            return grains.DocumentGrain.GetAsync();
        }
        public async Task<Result<DocumentState<JObject>, StorageResultReason>> GetCompactedAsync(Iri iri)
        {
            var grains = GetGrains(iri);
            var cacheResult = await grains.CompactedCacheGrain.TryGetValueAsync();
            if (cacheResult.HasValue)
                return new(cacheResult.Value);
            var raw = await grains.DocumentGrain.GetAsync();
            if (!raw.IsSuccessful)
                return new(raw.Reason);
            var compacted = raw.Value.Value != null
                ? await jsonLdService.CompactAsync(_instanceAuthorGrain, raw.Value.Value)
                : null;
            var result = new DocumentState<JObject>
            {
                BCc = raw.Value.BCc,
                BTo = raw.Value.BTo,
                IsReservation = raw.Value.IsReservation,
                Owner = raw.Value.Owner,
                UpdatedOnUtc = raw.Value.UpdatedOnUtc,
                Value = compacted
            };
            await grains.CompactedCacheGrain.SetValueAsync(result);
            return new(result);
        }

        public async Task<Result<DocumentState<JArray>, StorageResultReason>> GetExpandedAsync(Iri iri)
        {
            var grains = GetGrains(iri);
            var cacheResult = await grains.ExpandedCacheGrain.TryGetValueAsync();
            if (cacheResult.HasValue)
                return new(cacheResult.Value);
            var raw = await grains.DocumentGrain.GetAsync();
            if (!raw.IsSuccessful)
                return new(raw.Reason);
            var compacted = raw.Value.Value != null
                ? await jsonLdService.ExpandAsync(_instanceAuthorGrain, raw.Value.Value)
                : null;
            var result = new DocumentState<JArray>
            {
                BCc = raw.Value.BCc,
                BTo = raw.Value.BTo,
                IsReservation = raw.Value.IsReservation,
                Owner = raw.Value.Owner,
                UpdatedOnUtc = raw.Value.UpdatedOnUtc,
                Value = compacted
            };
            await grains.ExpandedCacheGrain.SetValueAsync(result);
            return new(result);
        }


        public async Task SetAsync(Iri iri, DocumentState value)
        {
            var grains = GetGrains(iri);
            await grains.CompactedCacheGrain.ClearAsync();
            await grains.ExpandedCacheGrain.ClearAsync();
            await grains.DocumentGrain.SetAsync(value);
        }
    }
}
