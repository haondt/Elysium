using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Elysium.Persistence.Services
{
    public class ElysiumMongoDbStorage : IElysiumStorage
    {
        private readonly IMongoCollection<MongoDbElysiumDocument> _collection;
        private readonly IMongoQueryable<MongoDbElysiumDocument> _queryableCollection;

        public ElysiumMongoDbStorage(
            IOptions<ElysiumPersistenceSettings> options,
            IMongoClient client)
        {
            var settings = options.Value;
            _collection = client.GetDatabase(settings.MongoDbStorageSettings.Database)
                .GetCollection<MongoDbElysiumDocument>(settings.MongoDbStorageSettings.Collection);
            _queryableCollection = _collection.AsQueryable();
        }

        public Task<bool> ContainsKey(StorageKey key)
        {
            var keyString = StorageKeyConvert.Serialize(key);
            return _queryableCollection
                .Where(q => q.PrimaryKey == key)
                .AnyAsync();
        }

        public async Task<Result<StorageResultReason>> Delete(StorageKey key)
        {
            var result = await _collection.DeleteOneAsync(q => q.PrimaryKey == key);
            return result.DeletedCount == 0 ? new(StorageResultReason.NotFound) : new();
        }

        public Task<Result<int, StorageResultReason>> DeleteMany<T>(StorageKey<T> foreignKey)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T, StorageResultReason>> Get<T>(StorageKey<T> key)
        {
            throw new NotImplementedException();
        }

        public Task<List<Result<object, StorageResultReason>>> GetMany(List<StorageKey> keys)
        {
            throw new NotImplementedException();
        }

        public Task<List<Result<T, StorageResultReason>>> GetMany<T>(List<StorageKey<T>> keys)
        {
            throw new NotImplementedException();
        }

        public Task<List<(StorageKey<T> Key, T Value)>> GetMany<T>(StorageKey<T> foreignKey)
        {
            throw new NotImplementedException();
        }

        public Task Set<T>(StorageKey<T> key, T value, List<StorageKey<T>> foreignKeys)
        {
            throw new NotImplementedException();
        }

        public Task Set<T>(StorageKey<T> key, T value)
        {
            var keyString = StorageKeyConvert.Serialize(key);
            return _collection.FindOneAndReplaceAsync<MongoDbElysiumDocument>(d => d.PrimaryKey == key, new MongoDbElysiumDocument
            {
                PrimaryKey = key,
                Value = value
            }, new FindOneAndReplaceOptions<MongoDbElysiumDocument, MongoDbElysiumDocument>
            {
                IsUpsert = true
            });
        }

        public Task SetMany(List<(StorageKey Key, object Value)> values)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<Result<T, StorageResultReason>>> GetMany<T>(List<StorageKey<T>> keys)
        //{
        //    var results = await GetMany(keys.Cast<StorageKey>().ToList());
        //    return results.Select(r =>
        //    {
        //        if (r.IsSuccessful)
        //            return new((T)r.Value);
        //        return new Result<T, StorageResultReason>(r.Reason);
        //    }).ToList();
        //}
    }
}
