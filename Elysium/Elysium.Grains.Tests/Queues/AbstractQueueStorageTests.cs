using Elysium.Grains.Queueing;
using FluentAssertions;

namespace Elysium.Grains.Tests.Queues
{
    public class TestData
    {
        public required string Value { get; set; }
    }

    public abstract class AbstractQueueStorageTests(Func<ITypedQueueStorageProvider> storageProviderFactory)
    {
        [Fact]
        public async Task WillCompleteBlockedDequeue()
        {
            var storageProvider = storageProviderFactory();
            var data = new TestData { Value = "some value" };
            var queueName = Guid.NewGuid().ToString();
            var storage = storageProvider.GetStorage<TestData>(queueName);

            TestData? dequeuedResult = null;
            _ = Task.Run(async () =>
            {
                var result = await storage.BlockingDequeueToStage();
                dequeuedResult = result.Payload;
                await storage.CommitDequeue(result.Key);
            });

            await Task.Delay(100);
            dequeuedResult.Should().BeNull();

            await storage.Enqueue(data);
            await Task.Delay(100);
            dequeuedResult.Should().NotBeNull();
            dequeuedResult!.Value.Should().BeEquivalentTo(data.Value);
        }

        [Fact]
        public async Task WillRecoverStagedDequeue()
        {
            var storageProvider = storageProviderFactory();
            var data = new TestData { Value = "some value" };
            var queueName = Guid.NewGuid().ToString();
            var storage = storageProvider.GetStorage<TestData>(queueName);

            await storage.Enqueue(data);
            await storage.BlockingDequeueToStage();

            var dequeueTask = storage.BlockingDequeueToStage();
            if (await Task.WhenAny(dequeueTask, Task.Delay(100)) == dequeueTask)
                Assert.Fail("task should have waited indefinitely");

            storageProvider = storageProviderFactory();
            storage = storageProvider.GetStorage<TestData>(queueName);
            var data2 = new TestData { Value = "some other value" };
            await storage.Enqueue(data2);
            var result = await dequeueTask;
            result.Payload.Value.Should().BeEquivalentTo(data2.Value);
            await storage.CommitDequeue(result.Key);

            await storage.RequeueDeadletters();
            result = await storage.BlockingDequeueToStage();
            result.Payload.Value.Should().BeEquivalentTo(data.Value);
            await storage.CommitDequeue(result.Key);
        }
    }
}
