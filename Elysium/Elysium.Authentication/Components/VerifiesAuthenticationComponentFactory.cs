using Elysium.Authentication.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;

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

        private void VerifyAuthentication(string componentIdentity)
        {
            if (_needsAuthenticationDescriptors.Contains(componentIdentity))
                if (!sessionService.IsAuthenticated())
                    throw new UnauthorizedAccessException();
        }

        public Task<IComponent> GetComponent(string componentIdentity, IComponentModel? model = null, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null)
        {
            VerifyAuthentication(componentIdentity);
            return inner.GetComponent(componentIdentity, model, configureResponse, requestData);
        }

        public Task<IComponent<T>> GetComponent<T>(T? model = default, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null) where T : IComponentModel
        {
            VerifyAuthentication(ComponentDescriptor<T>.TypeIdentity);
            return inner.GetComponent(model, configureResponse, requestData);
        }

        public Task<IComponent> GetPlainComponent<T>(T? model = default, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null) where T : IComponentModel
        {
            VerifyAuthentication(ComponentDescriptor<T>.TypeIdentity);
            return inner.GetPlainComponent(model, configureResponse, requestData);
        }
    }
}
