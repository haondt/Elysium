using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public interface IElysiumStorage : IStorage
    {
        Task<Result<UserIdentity, StorageResultReason>> GetUserByNameAsync(string normalizedUsername);
        Task SetMany(List<(StorageKey Key, object Value)> values);
        Task<List<Result<(StorageKey Key, object Value), StorageResultReason>>> GetMany(List<StorageKey> keys);
    }
}
