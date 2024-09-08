using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Models
{
    public class UserIdentity : IStorageKeyIdModel<UserIdentity>
    {
        public required StorageKey<UserIdentity> Id { get; set; }
        public string? PasswordHash { get; set; }
        public string? LocalizedUsername { get; set; }
        public required string EncryptedPrivateKey { get; set; }
        public required string PublicKey { get; set; }
        public string? NormalizedUsername { get; set; }
        public static StorageKey<UserIdentity> GetStorageKey(string username) => StorageKey<UserIdentity>.Create(username.ToLower().Trim());
    }
}
