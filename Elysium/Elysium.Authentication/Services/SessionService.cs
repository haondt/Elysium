﻿using Elysium.Authentication.Constants;
using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Elysium.Authentication.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly string _normalizedAdministratorRole;
        private Lazy<Task<Optional<StorageKey<UserIdentity>>>> _storageKeyLazy;
        private readonly Dictionary<string, string> _updatedCookies = [];

        public SessionService(IHttpContextAccessor httpContextAccessor, UserManager<UserIdentity> userManager, RoleManager<RoleIdentity> roleManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _normalizedAdministratorRole = roleManager.NormalizeKey(AuthenticationConstants.ADMINISTRATOR_ROLE);
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

        public bool IsAdministrator()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !IsAuthenticated())
                return false;

            var user = httpContext.User;
            return user.IsInRole(_normalizedAdministratorRole);
        }

        public Optional<T> GetFromCookie<T>(string key)
        {
            if (!_updatedCookies.TryGetValue(key, out var value))
            {
                var hasKey = _httpContextAccessor.HttpContext?.Request?.Cookies?.TryGetValue(key, out value);
                if (hasKey is null || hasKey == false || string.IsNullOrEmpty(value))
                    return new();
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(value);
                if (result is null)
                    return new();
                return new(result);
            }
            catch
            {
                return new();
            }
        }

        public void SetCookie<T>(string key, T value)
        {
            _updatedCookies[key] = JsonConvert.SerializeObject(value);
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(key, _updatedCookies[key]);
        }
    }
}
