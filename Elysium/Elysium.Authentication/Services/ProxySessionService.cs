using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class ProxySessionService(ISessionService sessionService) : ISessionService
    {
        public ISessionService SessionService { get; set; } = sessionService;
        public void ClearCache() => SessionService.ClearCache();
        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync() => SessionService.GetUserKeyAsync();
        public bool IsAuthenticated() => SessionService.IsAuthenticated();
    }
}
