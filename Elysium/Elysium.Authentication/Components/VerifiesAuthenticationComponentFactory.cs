using Elysium.Authentication.Services;
using Elysium.Components;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Components
{
    public class VerifiesAuthenticationComponentFactory(
        IComponentFactory inner,
        IEnumerable<IComponentDescriptor> descriptors,
        ISessionService sessionService) : IComponentFactory
    {
        private readonly HashSet<string> _needsAuthenticationDescriptors = descriptors
            .Where(d => d is INeedsAuthenticationComponentDescriptor)
            .Select(d => d.Identity)
            .ToHashSet();

        public Task<IComponent> GetComponent(string componentIdentity, IComponentModel? model = null, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null)
        {
            if (_needsAuthenticationDescriptors.Contains(componentIdentity))
                if (!sessionService.IsAuthenticated())
                    throw new UnauthorizedAccessException();
            return inner.GetComponent(componentIdentity, model, configureResponse, requestData);
        }

        public Task<IComponent<T>> GetComponent<T>(T? model = default, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null) where T : IComponentModel
        {
            if (_needsAuthenticationDescriptors.Contains(ComponentDescriptor<T>.TypeIdentity))
                if (!sessionService.IsAuthenticated())
                    throw new UnauthorizedAccessException();
            return inner.GetComponent(model, configureResponse, requestData);
        }

        public Task<IComponent> GetPlainComponent<T>(T? model = default, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null) where T : IComponentModel
        {
            if (_needsAuthenticationDescriptors.Contains(ComponentDescriptor<T>.TypeIdentity))
                if (!sessionService.IsAuthenticated())
                    throw new UnauthorizedAccessException();
            return inner.GetPlainComponent(model, configureResponse, requestData);
        }
    }
}
