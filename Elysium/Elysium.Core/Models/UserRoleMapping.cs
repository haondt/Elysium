using Haondt.Identity.StorageKey;

namespace Elysium.Core.Models
{
    public class UserRoleMapping
    {
        public static StorageKey<UserRoleMapping> GetStorageKey(StorageKey<UserIdentity> user, string roleName) => user.Extend<UserRoleMapping>(roleName);
        public static StorageKey<UserRoleMapping> GetForeignKey(StorageKey<UserIdentity> user) => user.Extend<UserRoleMapping>();
        public static StorageKey<UserRoleMapping> GetForeignKey(string roleName) => StorageKey<UserRoleMapping>.Create(roleName);
        public required StorageKey<UserIdentity> User { get; set; }
        public required string RoleName { get; set; }
    }
}
