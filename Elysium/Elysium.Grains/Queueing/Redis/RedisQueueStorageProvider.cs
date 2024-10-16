using Elysium.Core.Models;
using Elysium.Persistence.Converters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Elysium.Grains.Queueing.Redis
{
    public class RedisQueueStorageProvider : ITypedQueueStorageProvider
    {
        public QueueStorageType Type => QueueStorageType.Redis;
        private readonly ConcurrentDictionary<string, object> _queues = new();

        private static JsonSerializerSettings SerializerSettings = CreateSerializerSettings();
        private readonly IDatabase _blockingDatabase;
        private readonly IDatabase _nonblockingDatabase;

        public RedisQueueStorageProvider(
            IOptions<QueueSettings> options,
            IOptions<RedisSettings> redisOptions
        )
        {
            if (options.Value.Redis == null)
                throw new ArgumentNullException(nameof(QueueSettings.Redis));

            var multiplexer1 = ConnectionMultiplexer.Connect(redisOptions.Value.Endpoint);
            var multiplexer2 = ConnectionMultiplexer.Connect(redisOptions.Value.Endpoint);
            _blockingDatabase = multiplexer1.GetDatabase(options.Value.Redis.Database);
            _nonblockingDatabase = multiplexer2.GetDatabase(options.Value.Redis.Database);
        }

        public IQueueStorage<T> GetStorage<T>(string name)
        {
            var queueData = _queues.GetOrAdd(name, n => new QueueData<T>
            {
                Storage = new RedisQueueStorage<T>(_blockingDatabase, _nonblockingDatabase,
                n, SerializerSettings)
            });
            if (queueData is not QueueData<T> typedQueueData)
                throw new InvalidOperationException($"A queue with name {name} has already been added, but it has a type of {queueData.GetType()} instead of the expected {typeof(QueueData<T>)}");
            return typedQueueData.Storage;
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            settings.Formatting = Newtonsoft.Json.Formatting.None;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.Converters.Add(new GenericStorageKeyJsonConverter());
            return settings;
        }

        public class QueueData<T>
        {
            public required RedisQueueStorage<T> Storage { get; set; }
        }
    }
}
