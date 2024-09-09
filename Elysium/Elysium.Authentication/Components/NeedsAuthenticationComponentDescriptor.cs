using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Components
{
    public class NeedsAuthenticationComponentDescriptor : ComponentDescriptor, INeedsAuthenticationComponentDescriptor
    {
        public NeedsAuthenticationComponentDescriptor()
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, IRequestData, IComponentModel> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, IRequestData, Task<IComponentModel>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, IComponentModel> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, Task<IComponentModel>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentModel> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<Task<IComponentModel>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(IComponentModel defaultModel) : base(defaultModel)
        {
        }
    }
    public class NeedsAuthenticationComponentDescriptor<T> : ComponentDescriptor<T>, INeedsAuthenticationComponentDescriptor where T : IComponentModel
    {
        public NeedsAuthenticationComponentDescriptor()
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, IRequestData, T> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, IRequestData, Task<T>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, T> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<IComponentFactory, Task<T>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<T> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(Func<Task<T>> defaultModelFactory) : base(defaultModelFactory)
        {
        }

        public NeedsAuthenticationComponentDescriptor(T defaultModel) : base(defaultModel)
        {
        }
    }
}
