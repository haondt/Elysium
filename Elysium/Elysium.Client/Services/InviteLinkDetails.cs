using Haondt.Identity.StorageKey;

namespace Elysium.Client.Services
{
    public class InviteLinkDetails
    {
        public static StorageKey<InviteLinkDetails> GetStorageKey(string id) => StorageKey<InviteLinkDetails>.Create(id);
        public static (string Id, InviteLinkDetails Details) Create()
        {
            var stringId = Guid.NewGuid().ToString().Replace("-", "");
            return (stringId, new InviteLinkDetails());
        }

        public DateTime Expiry { get; set; } = DateTime.Now; // todo
    }
}
