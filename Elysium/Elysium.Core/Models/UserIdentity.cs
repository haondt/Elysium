using Haondt.Identity.StorageKey;

namespace Elysium.Core.Models
{
    [GenerateSerializer]
    public class UserIdentity : IStorageKeyIdModel<UserIdentity>
    {
        public static StorageKey<UserIdentity> GetStorageKey(string username) => StorageKey<UserIdentity>.Create(username.ToLower().Trim());
        public static StorageKey<NormalizedUsername> GetNormalizedUsernameStorageKey(string normalizedUsername) => StorageKey<NormalizedUsername>.Create(normalizedUsername);
        [Id(0)]
        public required StorageKey<UserIdentity> Id { get; set; }
        [Id(1)]
        public string? PasswordHash { get; set; }
        [Id(2)]
        public string? LocalizedUsername { get; set; }
        [Id(5)]
        public string? NormalizedUsername { get; set; }
    }

    public class NormalizedUsername
    {
        public static StorageKey<NormalizedUsername> GetStorageKey(string normalizedUsername) => StorageKey<NormalizedUsername>.Create(normalizedUsername);
    }
}
