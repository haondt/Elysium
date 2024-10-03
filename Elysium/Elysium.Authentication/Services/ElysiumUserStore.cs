using Elysium.Core.Models;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.Identity;

namespace Elysium.Authentication.Services
{
    // todo: use storagekeygrain instead of storage
    public class ElysiumUserStore(IElysiumStorage storage) : ElysiumStorageKeyIdModelStore<UserIdentity>(storage), IUserStore<UserIdentity>, IUserPasswordStore<UserIdentity>
    {
        public async Task<UserIdentity?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await _storage.GetMany(NormalizedUsername.GetStorageKey(normalizedUserName).Extend<UserIdentity>());
            if (result.Count == 1)
                return result.First().Value;
            return null;
        }

        public override async Task<IdentityResult> CreateAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            var hasUser = await _storage.ContainsKey(user.Id);
            if (hasUser)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "User already exists" });
            if (!string.IsNullOrEmpty(user.NormalizedUsername))
                await _storage.Set(user.Id, user, [NormalizedUsername.GetStorageKey(user.NormalizedUsername).Extend<UserIdentity>()]);
            else
                await _storage.Set(user.Id, user);
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(UserIdentity user, CancellationToken cancellationToken)
        {
            var hasUser = await _storage.ContainsKey(user.Id);
            if (!hasUser)
                return IdentityResult.Failed(new IdentityError { Code = "1", Description = "User does not exist" });
            if (!string.IsNullOrEmpty(user.NormalizedUsername))
                await _storage.Set(user.Id, user, [NormalizedUsername.GetStorageKey(user.NormalizedUsername).Extend<UserIdentity>()]);
            else
                await _storage.Set(user.Id, user);
            return IdentityResult.Success;
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
