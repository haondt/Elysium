using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;

namespace Elysium.Authentication.Services
{
    public interface ISessionService
    {
        public bool IsAuthenticated();
        public bool IsAdministrator();
        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync();
        public void ClearCache();
        Optional<T> GetFromCookie<T>(string key);
        void SetCookie<T>(string key, T value);
    }
}
