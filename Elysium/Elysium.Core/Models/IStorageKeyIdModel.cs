using Haondt.Identity.StorageKey;

namespace Elysium.Core.Models
{
    public interface IStorageKeyIdModel<T>
    {
        public StorageKey<T> Id { get; set; }
    }
}
