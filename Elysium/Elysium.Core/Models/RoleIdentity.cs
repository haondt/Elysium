using Haondt.Identity.StorageKey;

namespace Elysium.Core.Models
{
    public class RoleIdentity : IStorageKeyIdModel<RoleIdentity>
    {
        public required StorageKey<RoleIdentity> Id { get; set; }
        public static StorageKey<RoleIdentity> GetStorageKey(string role) => StorageKey<RoleIdentity>.Create(role.ToLower().Trim());
    }
}
