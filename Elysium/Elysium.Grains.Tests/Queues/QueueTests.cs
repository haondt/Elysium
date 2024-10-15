using Elysium.Grains.Tests.Queues.Grains;
using FluentAssertions;

namespace Elysium.Grains.Tests.Queues
{
    [Collection(QueueClusterCollection.Name)]
    public class QueueTests(ClusterFixture<QueueSiloConfigurator> fixture)
    {
        [Fact]
        public async Task WillEnqueueAndDequeuedItem()
        {
            var producer = fixture.Cluster.GrainFactory.GetGrain<IJobProducerGrain>(Guid.NewGuid());
            var consumer = fixture.Cluster.GrainFactory.GetGrain<IJobConsumerGrain>(Guid.Empty);
            await using var queue = await consumer.RentQueueAsync();

            await producer.ProduceJobAsync(queue.Name, "some payload");
            var next = await consumer.WatchQueueAsync(queue);
            next.Payload.Should().BeEquivalentTo("some payload");
        }

        [Fact]
        public async Task WillEnqueueAndDequeuedItemsInOrder()
        {
            // race conditions sometimes pass out of sheer luck
            for (int i = 0; i < 10; i++)
            {
                try
                {

                    var producer = fixture.Cluster.GrainFactory.GetGrain<IJobProducerGrain>(Guid.NewGuid());
                    var consumer = fixture.Cluster.GrainFactory.GetGrain<IJobConsumerGrain>(Guid.Empty);
                    await using var queue = await consumer.RentQueueAsync();

                    var payload1 = "abc";
                    var payload2 = "def";
                    var payload3 = "ghi";

                    // wait in between writes so the channel can be written to asynchronously
                    await producer.ProduceJobAsync(queue.Name, payload1);
                    await Task.Delay(10);
                    await producer.ProduceJobAsync(queue.Name, payload2);
                    await Task.Delay(10);
                    await producer.ProduceJobAsync(queue.Name, payload3);

                    var next = await consumer.WatchQueueAsync(queue);
                    next.Payload.Should().BeEquivalentTo(payload1);
                    next = await consumer.WatchQueueAsync(queue);
                    next.Payload.Should().BeEquivalentTo(payload2);
                    next = await consumer.WatchQueueAsync(queue);
                    next.Payload.Should().BeEquivalentTo(payload3);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed on iteration {i} with exception {ex}", ex);
                }
            }
        }

        [Fact]
        public async Task WillNotBottleneckOnSlowJobs()
        {
            var producer = fixture.Cluster.GrainFactory.GetGrain<IJobProducerGrain>(Guid.NewGuid());
            var consumer = fixture.Cluster.GrainFactory.GetGrain<IJobConsumerGrain>(Guid.Empty);
            await using var queue = await consumer.RentQueueAsync();

            var payload1 = "abc";
            var payload2 = "def";
            var payload3 = "ghi";

            var jobId2 = Guid.NewGuid();
            await consumer.BlockExecutionAsync(queue, jobId2);

            await producer.ProduceJobAsync(queue.Name, payload1);
            await Task.Delay(10);
            await producer.ProduceJobAsync(queue.Name, jobId2, payload2);
            await Task.Delay(10);
            await producer.ProduceJobAsync(queue.Name, payload3);

            var next = await consumer.WatchQueueAsync(queue);
            next.Payload.Should().BeEquivalentTo(payload1);
            next = await consumer.WatchQueueAsync(queue);
            next.Payload.Should().BeEquivalentTo(payload3);

            var nextTask = consumer.WatchQueueAsync(queue).AsTask();
            if (await Task.WhenAny(nextTask, Task.Delay(100)) == nextTask)
                Assert.Fail("task should have blocked indefinitely");

            await consumer.UnblockExecutionAsync(queue, jobId2);
            next = await nextTask;
            next.Payload.Should().BeEquivalentTo(payload2);
        }
    }
}
