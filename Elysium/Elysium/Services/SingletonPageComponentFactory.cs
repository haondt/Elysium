using DotNext;
using Haondt.Web.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Services;

namespace Elysium.Services
{
    public class SingletonPageComponentFactory(IServiceProvider serviceProvider) : ISingletonPageComponentFactory
    {
        public Task<T> ExecuteWithScopeAsync<T>(Func<IPageComponentFactory, Task<T>> func)
        {
            using var scope = serviceProvider.CreateScope();
            var componentFactory = scope.ServiceProvider.GetRequiredService<IPageComponentFactory>();
            return func(componentFactory);
        }

        public Task<Result<IComponent<PageModel>>> GetComponent(string targetComponentName)
            => ExecuteWithScopeAsync(f => f.GetComponent(targetComponentName));

        public Task<Result<IComponent<PageModel>>> GetComponent<T>() where T : IComponentModel
            => ExecuteWithScopeAsync(f => f.GetComponent<T>());

        public Task<Result<IComponent<PageModel>>> GetComponent(string targetComponentName, Dictionary<string, string> queryParams)
            => ExecuteWithScopeAsync(f => f.GetComponent(targetComponentName, queryParams));

        public Task<Result<IComponent<PageModel>>> GetComponent(string targetComponentName, List<(string Key, string Value)> queryParams)
            => ExecuteWithScopeAsync(f => f.GetComponent(targetComponentName, queryParams));

        public Task<Result<IComponent<PageModel>>> GetComponent<T>(Dictionary<string, string> queryParams) where T : IComponentModel
            => ExecuteWithScopeAsync(f => f.GetComponent<T>(queryParams));

        public Task<Result<IComponent<PageModel>>> GetComponent<T>(List<(string Key, string Value)> queryParams) where T : IComponentModel
            => ExecuteWithScopeAsync(f => f.GetComponent<T>(queryParams));
    }
}
