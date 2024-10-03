using Haondt.Identity.StorageKey;
using MongoDB.Bson.Serialization.Attributes;

namespace Elysium.Persistence.Services
{
    [BsonIgnoreExtraElements]
    public class MongoDbElysiumDocument
    {
        public required StorageKey PrimaryKey { get; set; }
        public List<string> ForeignKeys { get; set; } = [];
        public required object? Value { get; set; }
    }
}
