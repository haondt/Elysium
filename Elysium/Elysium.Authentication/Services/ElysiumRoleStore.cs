using Elysium.Core.Models;
using Elysium.Core.Services;
using Elysium.Persistence.Services;
using Haondt.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class ElysiumRoleStore(IElysiumStorage storage, IElysiumStorageKeyConverter converter) : ElysiumStorageKeyIdModelStore<RoleIdentity>(storage, converter), IRoleStore<RoleIdentity>
    {
        private readonly IElysiumStorageKeyConverter _converter = converter;

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
            return Task.FromResult(_converter.Serialize(role.Id));
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
