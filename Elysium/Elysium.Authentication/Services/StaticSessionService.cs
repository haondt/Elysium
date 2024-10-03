using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;

namespace Elysium.Authentication.Services
{
    public class StaticSessionService(Optional<StorageKey<UserIdentity>> identity) : ISessionService
    {
        public void ClearCache() { }

        public Optional<T> GetFromCookie<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync() => Task.FromResult(identity);

        public bool IsAuthenticated() => identity.HasValue;

        public void SetCookie<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
