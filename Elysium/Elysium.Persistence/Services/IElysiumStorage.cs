using DotNext;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public interface IElysiumStorage : IStorage
    {
        public Task<Result<UserIdentity>> GetUserByNameAsync(string normalizedUsername);
    }
}
