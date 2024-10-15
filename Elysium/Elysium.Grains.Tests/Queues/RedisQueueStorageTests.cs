using Elysium.Core.Models;
using Elysium.Grains.Queueing.Redis;
using Haondt.Identity.StorageKey;
using Microsoft.Extensions.Options;

namespace Elysium.Grains.Tests.Queues
{
    public class RedisQueueStorageTests : AbstractQueueStorageTests
    {
        public RedisQueueStorageTests() : base(() =>
            new RedisQueueStorageProvider(
                Options.Create(new RedisQueueSettings
                {
                    Database = 10
                }),
                Options.Create(new RedisSettings
                {
                    Endpoint = "localhost:6379"
                })))
        {
            StorageKeyConvert.DefaultSerializerSettings = new StorageKeySerializerSettings
            {
                TypeNameStrategy = TypeNameStrategy.SimpleTypeConverter,
                KeyEncodingStrategy = KeyEncodingStrategy.String
            };
        }

    }
}
