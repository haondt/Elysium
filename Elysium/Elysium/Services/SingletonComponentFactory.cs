using Haondt.Web.Core.Components;

namespace Elysium.Services
{
    public class SingletonComponentFactory(IServiceProvider serviceProvider) : ISingletonComponentFactory
    {
        public IComponentFactory CreateComponentFactory()
        {
            using var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IComponentFactory>();
        }
    }
}
