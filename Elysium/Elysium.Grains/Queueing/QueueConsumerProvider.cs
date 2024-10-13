using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Grains.Queueing
{
    public class QueueConsumerProvider(
        IServiceProvider serviceProvider) : IQueueConsumerProvider
    {
        public IQueueConsumer<T> GetConsumer<T>(string queue)
        {
            return serviceProvider.GetRequiredKeyedService<IQueueConsumer<T>>(queue);
        }
    }
}
