using Elysium.Authentication.Exceptions;
using Elysium.Authentication.Services;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Http;

namespace Elysium.Authentication.Components
{
    public class VerifiesAuthorizationComponentFactory(
        IComponentFactory inner,
        IEnumerable<IComponentDescriptor> descriptors,
        ISessionService sessionService) : IComponentFactory
    {
        private readonly Dictionary<string, INeedsAuthorizationComponentDescriptor> _needsAuthenticationDescriptors = descriptors
            .Where(d => d is INeedsAuthorizationComponentDescriptor)
            .Cast<INeedsAuthorizationComponentDescriptor>()
            .ToDictionary(d => d.Identity, d => d);

        private void VerifyAuthorization(string componentIdentity)
        {
            if (!_needsAuthenticationDescriptors.TryGetValue(componentIdentity, out var descriptor))
                return;

            foreach (var check in descriptor.AuthorizationChecks)
            {
                switch (check)
                {
                    case ComponentAuthorizationCheck.IsAuthenticated:
                        if (!sessionService.IsAuthenticated())
                            throw new NeedsAuthenticationException();
                        break;
                    case ComponentAuthorizationCheck.IsAdministrator:
                        if (!sessionService.IsAuthenticated())
                            throw new NeedsAuthenticationException();
                        if (!sessionService.IsAdministrator())
                            throw new NeedsAuthorizationException();
                        break;
                }
            }
        }

        public Task<IComponent> GetComponent(string componentIdentity, IComponentModel? model = null, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null)
        {
            VerifyAuthorization(componentIdentity);
            return inner.GetComponent(componentIdentity, model, configureResponse, requestData);
        }

        public Task<IComponent<T>> GetComponent<T>(T? model = default, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null) where T : IComponentModel
        {
            VerifyAuthorization(ComponentDescriptor<T>.TypeIdentity);
            return inner.GetComponent(model, configureResponse, requestData);
        }

        public Task<IComponent> GetPlainComponent<T>(T? model = default, Action<IHttpResponseMutator>? configureResponse = null, IRequestData? requestData = null) where T : IComponentModel
        {
            VerifyAuthorization(ComponentDescriptor<T>.TypeIdentity);
            return inner.GetPlainComponent(model, configureResponse, requestData);
        }
    }
}
