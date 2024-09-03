using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Models
{
    public class RoleIdentity : IStorageKeyIdModel<RoleIdentity>
    {
        public required StorageKey<RoleIdentity> Id { get; set; }
        public static StorageKey<RoleIdentity> GetStorageKey(string role) => StorageKey<RoleIdentity>.Create(role.ToLower().Trim());
    }
}
