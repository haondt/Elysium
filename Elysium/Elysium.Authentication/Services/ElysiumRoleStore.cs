using Elysium.Core.Models;
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
    public class ElysiumRoleStore(IStorage storage) : ElysiumStorageKeyIdModelStore<RoleIdentity>(storage), IRoleStore<RoleIdentity>
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
