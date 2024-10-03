using Elysium.Core.Models;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.Identity;

namespace Elysium.Authentication.Services
{
    public class ElysiumRoleStore(IElysiumStorage storage) : ElysiumStorageKeyIdModelStore<RoleIdentity>(storage), IRoleStore<RoleIdentity>
    {
        public Task<RoleIdentity?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string?> GetNormalizedRoleNameAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetRoleIdAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(StorageKeyConvert.Serialize(role.Id));
        }

        public Task<string?> GetRoleNameAsync(RoleIdentity role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task SetNormalizedRoleNameAsync(RoleIdentity role, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task SetRoleNameAsync(RoleIdentity role, string? roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
