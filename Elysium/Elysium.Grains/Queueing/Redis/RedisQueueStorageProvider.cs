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

        private static readonly JsonSerializerSettings SerializerSettings = CreateSerializerSettings();
        private readonly RedisSettings _redisSettings;
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly QueueSettings _queueSettings;
        private readonly IDatabase _sharedDatabase;

        public RedisQueueStorageProvider(
            IOptions<QueueSettings> options,
            IOptions<RedisSettings> redisOptions
        )
        {
            _redisSettings = redisOptions.Value;
            _multiplexer = ConnectionMultiplexer.Connect(_redisSettings.Endpoint);
            var sharedMultiplexer = ConnectionMultiplexer.Connect(_redisSettings.Endpoint);
            _queueSettings = options.Value;
            _sharedDatabase = sharedMultiplexer.GetDatabase(_queueSettings.Redis.Database);
        }

        public IQueueStorage<T> GetStorage<T>(string name)
        {
            var queueData = _queues.GetOrAdd(name, n => new RedisQueueStorage<T>(_multiplexer,
                _queueSettings.Redis.ChannelDiscriminator,
                n,
                _queueSettings.Redis.Database,
                SerializerSettings));

            if (queueData is not RedisQueueStorage<T> typedQueueData)
                throw new InvalidOperationException($"A queue with name {name} has already been added, but it has a type of {queueData.GetType()} instead of the expected {typeof(RedisQueueStorage<T>)}");
            return typedQueueData;
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error,
                Formatting = Newtonsoft.Json.Formatting.None,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };
            settings.Converters.Add(new GenericStorageKeyJsonConverter());
            return settings;
        }

    }
}
