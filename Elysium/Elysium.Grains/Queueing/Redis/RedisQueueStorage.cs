using Haondt.Identity.StorageKey;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Elysium.Grains.Queueing.Redis
{
    public class RedisQueueStorage<T> : IQueueStorage<T>
    {
        private readonly string _queueName;
        private readonly string _stageQueue;
        private readonly IDatabase _database;
        private readonly Channel<StorageKey<T>> _watcherChannel = Channel.CreateUnbounded<StorageKey<T>>();
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly RedisChannel _channel;
        private readonly ISubscriber _subscriber;
        private ConcurrentQueue<TaskCompletionSource> _waitingForNextEnqueue = new();

        public RedisQueueStorage(
            ConnectionMultiplexer redis,
            string channelDiscriminator,
            string queueName,
            int database,
            JsonSerializerSettings serializerSettings)
        {
            _queueName = queueName;
            _stageQueue = $"{queueName}:stage";
            _database = redis.GetDatabase(database);
            var channelName = $"{channelDiscriminator}{_queueName}";
            _channel = RedisChannel.Literal(channelName);
            _subscriber = redis.GetSubscriber();
            _subscriber.Subscribe(_channel, (channel, message) => OnEnqueue());
            _serializerSettings = serializerSettings;
        }


        public async Task<(StorageKey<T> Key, T Payload)> BlockingDequeueToStage()
        {
            var result = await _database.ListRightPopLeftPushAsync(_queueName, _stageQueue);
            while (result.IsNullOrEmpty)
            {
                await WaitForNextEnqueueAsync();
                result = await _database.ListRightPopLeftPushAsync(_queueName, _stageQueue);
            }

            var key = StorageKeyConvert.Deserialize<T>(result.ToString());
            var payload = JsonConvert.DeserializeObject<T>(key.Parts[^1].Value)
                ?? throw new InvalidOperationException($"Failed to deserialize item from queue {_queueName}");
            return (key, payload);
        }

        public Task WaitForNextEnqueueAsync()
        {
            var completionSource = new TaskCompletionSource();
            _waitingForNextEnqueue.Enqueue(completionSource);
            return completionSource.Task;
        }

        private void OnEnqueue()
        {
            var toComplete = new List<TaskCompletionSource>();
            while (_waitingForNextEnqueue.TryDequeue(out var waitingTask))
                toComplete.Add(waitingTask);

            foreach (var waitingTask in toComplete)
                waitingTask.SetResult();
        }

        public Task CommitDequeue(StorageKey<T> key)
        {
            return _database.ListRemoveAsync(_stageQueue, StorageKeyConvert.Serialize(key), 1);
        }

        public async Task<StorageKey<T>> Enqueue(T payload)
        {
            var serializedPayload = JsonConvert.SerializeObject(payload, _serializerSettings);
            var key = StorageKey<Guid>.Create(Guid.NewGuid().ToString()).Extend<T>(serializedPayload);
            await _database.ListRightPushAsync(_queueName, StorageKeyConvert.Serialize(key));
            await _subscriber.PublishAsync(_channel, "");
            return key;
        }

        public async Task RequeueDeadletters()
        {
            while (true)
            {
                var result = await _database.ListRightPopAsync(_stageQueue);
                if (!result.HasValue)
                    break;

                var key = StorageKeyConvert.Deserialize<T>(result.ToString());
                var payload = JsonConvert.DeserializeObject<T>(key.Parts[^1].Value);
                if (payload == null)
                    continue; // todo: log

                await Enqueue(payload);
            }

        }

    }
}
