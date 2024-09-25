using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public interface IElysiumStorage : IStorage
    {
        Task SetMany(List<(StorageKey Key, object Value)> values);
        Task<List<Result<(StorageKey Key, object Value), StorageResultReason>>> GetMany(List<StorageKey> keys);

        Task Set<T>(StorageKey<T> key, T value, List<StorageKey> foreignKeys);
        Task<List<(StorageKey<TPrimary> Key, TPrimary Value)>> Get<TPrimary, TForeign>(StorageKey<TPrimary> partialPrimaryKey, StorageKey<TForeign> foreignKey);
        Task<Result<int, StorageResultReason>> DeleteMany<TPrimary, TForegin>(StorageKey<TPrimary> partialPrimaryKey, StorageKey<TForegin> foreignKey);
    }
}
