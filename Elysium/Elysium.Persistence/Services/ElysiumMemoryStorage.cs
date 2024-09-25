using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public class MemoryEntry
    {
        public required object Value { get; set; }
        public HashSet<StorageKey> ForeignKeys { get; set; } = [];
    }

    public class ElysiumMemoryStorage : IElysiumStorage
    {
        protected readonly Dictionary<StorageKey, MemoryEntry> _storage = new();

        public Task<bool> ContainsKey(StorageKey key) => Task.FromResult(_storage.ContainsKey(key));

        public Task<Result<StorageResultReason>> Delete(StorageKey key)
        {
            if (_storage.Remove(key))
                return Task.FromResult(new Result<StorageResultReason>());
            return Task.FromResult(new Result<StorageResultReason>(StorageResultReason.NotFound));
        }

        public Task<List<Result<StorageResultReason>>> DeleteMany<TPrimary, TForegin>(StorageKey<TForegin> foreignKey)
        {
            return Task.FromResult(_storage.Select(kvp =>
            {
                if (kvp.Value.ForeignKeys.Contains(foreignKey))
                {
                    _storage.Remove(kvp.Key);
                    return new Result<StorageResultReason>();
                }
                return new Result<StorageResultReason>(StorageResultReason.NotFound);
            }).ToList());
        }

        public Task<Result<int, StorageResultReason>> DeleteMany<TPrimary, TForegin>(StorageKey<TPrimary> partialPrimaryKey, StorageKey<TForegin> foreignKey)
        {
            var keysToRemove = _storage.Where(kvp => kvp.Value.ForeignKeys.Contains(foreignKey))
                .Select(kvp => kvp.Key);
            var removed = 0;
            foreach(var key in keysToRemove)
            {
                _storage.Remove(key);
                removed++;
            }

            return Task.FromResult<Result<int, StorageResultReason>>(removed > 0 ? new(removed) : new(StorageResultReason.NotFound));
        }


        public Task<Result<T, StorageResultReason>> Get<T>(StorageKey<T> key)
        {
            if (_storage.TryGetValue(key, out var value))
                return Task.FromResult(new Result<T, StorageResultReason>((T)value.Value));
            return Task.FromResult(new Result<T, StorageResultReason>(StorageResultReason.NotFound));
        }

        public Task<List<(StorageKey<TPrimary> Key, TPrimary Value)>> Get<TPrimary, TForeign>(StorageKey<TPrimary> partialPrimaryKey, StorageKey<TForeign> foreignKey)
        {
            return Task.FromResult(_storage
                .Where(kvp => kvp.Key.Type == typeof(TPrimary))
                .Where(kvp => kvp.Value.ForeignKeys.Contains(foreignKey))
                .Select(kvp => (kvp.Key.As<TPrimary>(), (TPrimary)kvp.Value.Value))
                .ToList());
        }

        public Task<List<Result<(StorageKey Key, object Value), StorageResultReason>>> GetMany(List<StorageKey> keys)
        {
            return Task.FromResult(keys.Select(k =>
            {
                if (_storage.TryGetValue(k, out var value) && value.Value.GetType() == k.Type)
                    return new((k, value.Value));
                return new Result<(StorageKey, object), StorageResultReason>(StorageResultReason.NotFound);
            }).ToList());
        }


        public Task Set<T>(StorageKey<T> key, T value, List<StorageKey> foreignKeys)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            _storage[key] = new MemoryEntry
            {
                Value = value,
                ForeignKeys = foreignKeys.ToHashSet()
            };
            return Task.CompletedTask;
        }

        public Task Set<T>(StorageKey<T> key, T value) => Set(key, value, []);

        public Task SetMany(List<(StorageKey Key, object Value)> values)
        {
            foreach (var (key, value) in values)
                _storage[key] = new MemoryEntry { Value = value };
            return Task.CompletedTask;
        }

    }
}
