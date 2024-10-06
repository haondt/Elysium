using Elysium.Core.Models;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.Identity;

namespace Elysium.Authentication.Services
{
    public class ElysiumRoleStore(IElysiumStorage storage) : ElysiumStorageKeyIdModelStore<RoleIdentity>(storage), IRoleStore<RoleIdentity>
    {
        public async Task<RoleIdentity?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var result = await _storage.GetMany(NormalizedRoleName.GetStorageKey(normalizedRoleName).Extend<RoleIdentity>());
            if (result.Count == 1)
                return result.First().Value;
            return null;
        }

        public override async Task<IdentityResult> CreateAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            if (await _storage.ContainsKey(role.Id))
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "Role already exists" });
            if (!string.IsNullOrEmpty(role.NormalizedName))
                await _storage.Set(role.Id, role, [NormalizedRoleName.GetStorageKey(role.NormalizedName).Extend<RoleIdentity>()]);
            else
                await _storage.Set(role.Id, role);
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            if (!await _storage.ContainsKey(role.Id))
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "Role does not exist" });
            if (!string.IsNullOrEmpty(role.NormalizedName))
                await _storage.Set(role.Id, role, [NormalizedRoleName.GetStorageKey(role.NormalizedName).Extend<RoleIdentity>()]);
            else
                await _storage.Set(role.Id, role);
            return IdentityResult.Success;
        }

        public Task<string?> GetNormalizedRoleNameAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(StorageKeyConvert.Serialize(role.Id));
        }

        public Task<string?> GetRoleNameAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(RoleIdentity role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(RoleIdentity role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }
    }
}
