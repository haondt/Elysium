using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;

namespace Elysium.Persistence.Services
{
    public interface IElysiumStorage : IStorage
    {
        Task SetMany(List<(StorageKey Key, object? Value)> values);
        Task<List<Result<object?, StorageResultReason>>> GetMany(List<StorageKey> keys);
        Task<List<Result<T, StorageResultReason>>> GetMany<T>(List<StorageKey<T>> keys);

        Task Set<T>(StorageKey<T> key, T value, List<StorageKey<T>> addForeignKeys);
        Task<List<(StorageKey<T> Key, T Value)>> GetMany<T>(StorageKey<T> foreignKey);
        Task<Result<int, StorageResultReason>> DeleteMany<T>(StorageKey<T> foreignKey);
    }
}
