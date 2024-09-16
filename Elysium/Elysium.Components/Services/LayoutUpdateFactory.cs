using Haondt.Web.Core.Components;
using Haondt.Web.Services;

namespace Elysium.Components.Services
{
    public class LayoutUpdateFactory(IComponentFactory componentFactory) : ILayoutUpdateFactory
    {
        private Task<IComponent>? _buildAsync;
        public Task<IComponent> BuildAsync() => _buildAsync ?? throw new InvalidOperationException("build not set");

        public ILayoutUpdateFactory GetInitialLayout(IComponent content)
        {
            _buildAsync = componentFactory.GetPlainComponent(new Components.DefaultLayoutModel
            {
                Content = content,
            });
            return this;
        }

        public ILayoutUpdateFactory SetContent(IComponent component)
        {
            throw new NotSupportedException();
        }
    }

}
