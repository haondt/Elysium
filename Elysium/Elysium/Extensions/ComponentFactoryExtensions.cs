using Haondt.Web.Core.Components;

namespace Elysium.Extensions
{
    public static class ComponentFactoryExtensions
    {
        public static Task<IComponent> GetPlainComponentWithStatusCode<T>(this IComponentFactory componentFactory, T model, int statusCode) where T : IComponentModel
        {
            return componentFactory.GetPlainComponent(model, configureResponse: m => m.SetStatusCode = statusCode);
        }
    }
}
