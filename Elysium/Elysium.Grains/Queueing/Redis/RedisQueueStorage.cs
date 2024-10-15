using Haondt.Identity.StorageKey;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Elysium.Grains.Queueing.Redis
{
    public class RedisQueueStorage<T>(
        IDatabase blockingDatabase,
        IDatabase nonBlockingDatabase,
        string queueName,
        JsonSerializerSettings serializerSettings) : IQueueStorage<T>
    {
        private readonly string _stageQueue = $"{queueName}:stage";
        public async Task<(StorageKey<T> Key, T Payload)> BlockingDequeueToStage()
        {
            var result = await blockingDatabase.ExecuteAsync("BRPOPLPUSH", queueName, _stageQueue, 0);
            if (result.IsNull)
                throw new InvalidOperationException("No items in the queue.");

            var key = StorageKeyConvert.Deserialize<T>(result.ToString());
            var payload = JsonConvert.DeserializeObject<T>(key.Parts[^1].Value)
                ?? throw new InvalidOperationException($"Failed to deserialize item from queue {queueName}");
            return (key, payload);
        }

        public Task CommitDequeue(StorageKey<T> key)
        {
            return nonBlockingDatabase.ListRemoveAsync(_stageQueue, StorageKeyConvert.Serialize(key), 1);
        }

        public async Task<StorageKey<T>> Enqueue(T payload)
        {
            var serializedPayload = JsonConvert.SerializeObject(payload, serializerSettings);
            var key = StorageKey<Guid>.Create(Guid.NewGuid().ToString()).Extend<T>(serializedPayload);
            await nonBlockingDatabase.ListRightPushAsync(queueName, StorageKeyConvert.Serialize(key));
            return key;
        }

        public async Task RequeueDeadletters()
        {
            while (true)
            {
                var result = await nonBlockingDatabase.ListRightPopAsync(_stageQueue);
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
