using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;

namespace Elysium.Authentication.Components
{
    public class NeedsAuthorizationComponentDescriptor : ComponentDescriptor, INeedsAuthorizationComponentDescriptor
    {
        public List<ComponentAuthorizationCheck> AuthorizationChecks { get; set; } = [];

        public NeedsAuthorizationComponentDescriptor()
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, IRequestData, IComponentModel> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, IRequestData, Task<IComponentModel>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, IComponentModel> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, Task<IComponentModel>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentModel> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<Task<IComponentModel>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(IComponentModel defaultModel) : base(defaultModel)
        {
        }
    }
    public class NeedsAuthorizationComponentDescriptor<T> : ComponentDescriptor<T>, INeedsAuthorizationComponentDescriptor where T : IComponentModel
    {
        public List<ComponentAuthorizationCheck> AuthorizationChecks { get; set; } = [];

        public NeedsAuthorizationComponentDescriptor()
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, IRequestData, T> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, IRequestData, Task<T>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, T> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<IComponentFactory, Task<T>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<T> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(Func<Task<T>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthorizationComponentDescriptor(T defaultModel) : base(defaultModel)
        {
        }
    }
}
