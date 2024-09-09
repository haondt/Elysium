using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserIdentity> _userManager;
        private Lazy<Task<Optional<StorageKey<UserIdentity>>>> _storageKeyLazy;

        public SessionService(IHttpContextAccessor httpContextAccessor, UserManager<UserIdentity> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            ClearCache();
        }

        [MemberNotNull(nameof(_storageKeyLazy))]
        public void ClearCache()
        {
            _storageKeyLazy = new(async () =>
            {
                if (_httpContextAccessor.HttpContext is null)
                    return new();

                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userId is null)
                    return new();

                var user = await _userManager.FindByIdAsync(userId.Value);
                if (user is null)
                    return new();
                return new(user.Id);
            });
        }

        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync() => _storageKeyLazy.Value;

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
        }

    }
}
