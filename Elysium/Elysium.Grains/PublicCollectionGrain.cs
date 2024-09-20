using Elysium.ActivityPub.Models;
using Elysium.Core.Converters;
using Elysium.Core.Models;
using Elysium.Domain;
using Elysium.GrainInterfaces;
using Elysium.Persistence.Exceptions;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class PublicCollectionGrain : Grain, IPublicCollectionGrain
    {
        private readonly TimeSpan _cacheDuration;
        private PublicCollectionState _state = new();
        private readonly IMemoryCache _cache;
        private readonly IElysiumStorage _elysiumStorage;
        private StorageKey<PublicCollectionState> _baseKey;

        public PublicCollectionGrain(IOptions<PublicCollectionSettings> options,
            IMemoryCache cache,
            IElysiumStorage elysiumStorage)
        {
            _cache = cache;
            _elysiumStorage = elysiumStorage;
            _cacheDuration = TimeSpan.FromMinutes(options.Value.CacheDurationInMinutes);
            _baseKey = PublicCollectionState.GetStorageKey(this.GetPrimaryKey());
        }


        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var result = await _elysiumStorage.Get(_baseKey);
            if (result.IsSuccessful)
                _state = result.Value;
            else if (result.Reason != StorageResultReason.NotFound)
                throw new StorageException($"Failed to retrieve {nameof(PublicCollectionState)} with key {_baseKey} from storage");
            await base.OnActivateAsync(cancellationToken);
        }

        // todo: maybe implement a buffer here to not bottleneck the ingestion stream
        public async Task IngestReferenceAsync(ActivityType activityType, Iri iri)
        {
            var reference = new CollectionDocumentReference
            {
                Iri = iri.ToString(),
            };
            var stateCopy = new PublicCollectionState
            {
                Lasts = _state.Lasts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
            if (!stateCopy.Lasts.ContainsKey(activityType))
                stateCopy.Lasts[activityType] = 0;
            else
                stateCopy.Lasts[activityType]++;

            await _elysiumStorage.SetMany(
            [
                (_baseKey.Extend<CollectionDocumentReference>(LongConverter.EncodeLong(stateCopy.Lasts[activityType])), reference),
                (_baseKey, stateCopy)
            ]);

            _cache.Set((activityType, stateCopy.Lasts[activityType]), reference, _cacheDuration);
            _state = stateCopy;
        }

        /// <summary>
        /// Get <paramref name="count"/> references from before <paramref name="before"/>
        /// e.g. (before: 16, count: 5) -> [15, 14, 13, 12, 11]
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<CollectionResult> GetReferencesAsync(ActivityType activityType, long before, int count)
        {
            if (!_state.Lasts.TryGetValue(activityType, out var last))
                return new CollectionResult { References = [], Last = -1 };
            if (before <= 0)
                return new CollectionResult { References = [], Last = -1 };
            if (count <= 0)
                return new CollectionResult { References = [], Last = -1 };

            var start = before - 1;
            if (start > last)
                start = last;

            var end = start - (count - 1);
            if (end < 0)
                end = 0;

            List<string?> found = new((int)(start - end));
            List<long> missing = [];

            for (long i = start; i >= end; i--)
                if (_cache.TryGetValue((activityType, i), out var foundObject) && foundObject is CollectionDocumentReference reference)
                    found.Add(reference.Iri);
                else
                {
                    missing.Add(i);
                    found.Add(null);
                }

            var missingLookup = await _elysiumStorage
                .GetMany(missing.Select(i => (StorageKey)_baseKey.Extend<CollectionDocumentReference>(LongConverter.EncodeLong(i))).ToList());

            foreach (var (i, lookup) in missing.Zip(missingLookup))
                if (lookup.IsSuccessful && lookup.Value.Value is CollectionDocumentReference reference)
                    found[(int)(start - i)] = reference.Iri;

            var references = found.Where(r => !string.IsNullOrEmpty(r))
                .Cast<string>()
                .Select(Iri.FromUnencodedString).ToList();
            return new CollectionResult { References = references, Last = last };
        }

        /// <summary>
        /// Get the <paramref name="count"/> most recent references
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<CollectionResult> GetReferencesAsync(ActivityType activityType, int count) => 
            GetReferencesAsync(activityType, _state.Lasts.TryGetValue(activityType, out var last) ? last + 1 : 0, count);
    }

    public class PublicCollectionState
    {
        public static StorageKey<PublicCollectionState> GetStorageKey(Guid id) => StorageKey<PublicCollectionState>.Create(id.ToString());
        public Dictionary<ActivityType, long> Lasts = []; 
    }
}
