using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public class ElysiumMemoryStorage : MemoryStorage, IElysiumStorage
    {
        public Task<List<Result<(StorageKey Key, object Value), StorageResultReason>>> GetMany(List<StorageKey> keys)
        {
            return Task.FromResult(keys.Select(k =>
            {
                if (_storage.TryGetValue(k, out var value) && value.GetType() == k.Type)
                    return new((k, value));
                return new Result<(StorageKey, object), StorageResultReason>(StorageResultReason.NotFound);
            }).ToList());
        }

        public Task<Result<UserIdentity, StorageResultReason>> GetUserByNameAsync(string normalizedUsername)
        {
            foreach (var obj in _storage.Values)
                if (obj is UserIdentity userIdentity && userIdentity.NormalizedUsername == normalizedUsername)
                    return Task.FromResult<Result<UserIdentity, StorageResultReason>>(new(userIdentity));
            return Task.FromResult<Result<UserIdentity, StorageResultReason>>(new(StorageResultReason.NotFound));
        }

        public Task SetMany(List<(StorageKey Key, object Value)> values)
        {
            foreach(var (key, value) in values)
            {
                _storage[key] = value;
            }

            return Task.CompletedTask;
        }
    }
}
