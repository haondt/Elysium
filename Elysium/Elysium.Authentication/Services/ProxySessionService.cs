using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;

namespace Elysium.Authentication.Services
{
    public class ProxySessionService(ISessionService sessionService) : ISessionService
    {
        public ISessionService SessionService { get; set; } = sessionService;
        public void ClearCache() => SessionService.ClearCache();

        public Optional<T> GetFromCookie<T>(string key) => SessionService.GetFromCookie<T>(key);

        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync() => SessionService.GetUserKeyAsync();

        public bool IsAdministrator() => SessionService.IsAdministrator();

        public bool IsAuthenticated() => SessionService.IsAuthenticated();

        public void SetCookie<T>(string key, T value) => SessionService.SetCookie<T>(key, value);
    }
}
