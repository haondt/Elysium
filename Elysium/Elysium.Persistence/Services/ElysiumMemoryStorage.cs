using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public class ElysiumMemoryStorage : MemoryStorage, IElysiumStorage
    {
        public Task<Result<UserIdentity, StorageResultReason>> GetUserByNameAsync(string normalizedUsername)
        {
            foreach (var obj in _storage.Values)
                if (obj is UserIdentity userIdentity && userIdentity.NormalizedUsername == normalizedUsername)
                    return Task.FromResult<Result<UserIdentity, StorageResultReason>>(new(userIdentity));
            return Task.FromResult<Result<UserIdentity, StorageResultReason>>(new(StorageResultReason.NotFound));
        }
    }
}
