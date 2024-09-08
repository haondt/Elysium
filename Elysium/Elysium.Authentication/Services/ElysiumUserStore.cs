using Elysium.Core.Models;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class ElysiumUserStore(IElysiumStorage storage) : ElysiumStorageKeyIdModelStore<UserIdentity>(storage), IUserStore<UserIdentity>, IUserPasswordStore<UserIdentity>
    {
        public async Task<UserIdentity?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await storage.GetUserByNameAsync(normalizedUserName);
            if (result.IsSuccessful)
                return result.Value;
            return null;
        }

        public Task<string?> GetNormalizedUserNameAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUsername);
        }

        public Task<string?> GetPasswordHashAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(StorageKeyConvert.Serialize(user.Id));
        }

        public Task<string?> GetUserNameAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LocalizedUsername);
        }

        public Task<bool> HasPasswordAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetNormalizedUserNameAsync(UserIdentity user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUsername = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UserIdentity user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(UserIdentity user, string? userName, CancellationToken cancellationToken)
        {
            user.LocalizedUsername = userName;
            return Task.CompletedTask;
        }
    }
}
