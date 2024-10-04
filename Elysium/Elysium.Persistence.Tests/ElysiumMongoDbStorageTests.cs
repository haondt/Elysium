using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.MongoDb.Converters;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

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

        private static MongoClientSettings MongoClientSettings { get
            {

                var settings = MongoClientSettings.FromConnectionString("mongodb://elysium:elysium@localhost:27017/");
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        var x = e.CommandName;
                        var y = e.Command.ToJson();
                        ;
                    });
                };
                return settings;
            } }

        public ElysiumMongoDbStorageTests() : base(
            new ElysiumMongoDbStorage(
                Options.Create(new ElysiumPersistenceSettings
                {
                    Driver = ElysiumPersistenceDrivers.MongoDb,
                    MongoDbStorageSettings = new ElysiumMongoDbStorageSettings
                    {
                        Collection = "test",
                        StoreKeyStrings = true,
                        ConnectionString = "",
                    }
                }), new MongoClient(MongoClientSettings)))
        {
            StorageKeyConvert.DefaultSerializerSettings = new StorageKeySerializerSettings
            {
                TypeNameStrategy = TypeNameStrategy.SimpleTypeConverter,
                KeyEncodingStrategy = KeyEncodingStrategy.String
            };
        }
    }
}
