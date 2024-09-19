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
        public async Task IngestReferenceAsync(string iri)
        {
            var reference = new CollectionDocumentReference
            {
                Iri = iri,
                Previous = _state.Last
            };
            var stateCopy = new PublicCollectionState
            {
                Last = _state.Last + 1
            };

            await _elysiumStorage.SetMany(
            [
                (_baseKey.Extend<CollectionDocumentReference>($"{stateCopy.Last}"), reference),
                (_baseKey, stateCopy)
            ]);

            _cache.Set(stateCopy.Last, reference, _cacheDuration);
            _state = stateCopy;
        }

        /// <summary>
        /// Get references from - to. This returns items in the most recent order, so
        /// items from 5 - 15 would 5, 6, 7, ..., 15, where 5 is the newest entry. 0-indexed.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetReferencesAsync(int from, int to)
        {
            var start = _state.Last;
            if (start == -1)
                return [];
            var end = to >= _state.Last ? 0 : _state.Last - to;

            List<string?> found = new((end - start) + 1);
            List<int> missing = [];

            for(int i = start; i <= end; i++)
                if (_cache.TryGetValue(i, out var foundObject) && foundObject is CollectionDocumentReference reference)
                    found.Add(reference.Iri);
                else
                {
                    missing.Add(i);
                    found.Add(null);
                }

            var missingLookup = await _elysiumStorage
                .GetMany(missing.Select(i => (StorageKey)_baseKey.Extend<CollectionDocumentReference>($"{i}")).ToList());

            foreach(var (i, lookup) in missing.Zip(missingLookup))
                if (lookup.IsSuccessful && lookup.Value.Value is CollectionDocumentReference reference)
                    found[i] = reference.Iri;

            return found.Where(s => !string.IsNullOrEmpty(s)).Cast<string>();
        }

        /// <summary>
        /// Get the <paramref name="count"/> most recent references
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetReferencesAsync(int count) => GetReferencesAsync(0, 0 + count);

    }

    public class PublicCollectionState
    {
        public static StorageKey<PublicCollectionState> GetStorageKey(Guid id) => StorageKey<PublicCollectionState>.Create(id.ToString());
        public int Last { get; set; } = -1;
    }
}
