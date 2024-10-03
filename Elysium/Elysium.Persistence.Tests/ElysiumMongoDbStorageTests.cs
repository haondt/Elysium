using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.MongoDb.Converters;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Elysium.Persistence.Tests
{
    public class ElysiumMongoDbStorageTests : AbstractElysiumStorageTests
    {
        static ElysiumMongoDbStorageTests()
        {
            BsonSerializer.RegisterSerializer(new ObjectSerializer(type => true));
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(StorageKey<>), typeof(StorageKeyBsonConverter<>));
            BsonSerializer.RegisterSerializer(typeof(StorageKey), new StorageKeyBsonConverter());
        }

        public ElysiumMongoDbStorageTests() : base(
            new ElysiumMongoDbStorage(
                Options.Create(new ElysiumPersistenceSettings
                {
                    Driver = ElysiumPersistenceDrivers.MongoDb,
                    MongoDbStorageSettings = new ElysiumMongoDbStorageSettings
                    {
                        Collection = "test",
                        StoreKeyStrings = true
                    }
                }), new MongoClient("mongodb://elysium:elysium@localhost:27017/")))
        {
            StorageKeyConvert.DefaultSerializerSettings = new StorageKeySerializerSettings
            {
                TypeNameStrategy = TypeNameStrategy.SimpleTypeConverter,
                KeyEncodingStrategy = KeyEncodingStrategy.String
            };
        }
    }
}
