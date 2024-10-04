using Haondt.Identity.StorageKey;
using MongoDB.Bson.Serialization.Attributes;

namespace Elysium.Persistence.Services
{
    [BsonIgnoreExtraElements]
    public class MongoDbElysiumDocument
    {
        //[BsonId]
        public required StorageKey PrimaryKey { get; set; }
        public List<StorageKey> ForeignKeys { get; set; } = [];
        public required object? Value { get; set; }
    }
}
