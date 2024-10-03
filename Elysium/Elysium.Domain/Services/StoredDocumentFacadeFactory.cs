using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Domain.Services
{
    /// <summary>
    /// This factory is just to break the dependency cycle between <see cref="DocumentService"/>
    /// and <see cref="JsonLdService"/>.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public class StoredDocumentFacadeFactory(IServiceProvider serviceProvider) : IStoredDocumentFacadeFactory
    {
        public IStoredDocumentFacade Create(IDocumentService documentService)
        {
            var jsonLdService = ActivatorUtilities.CreateInstance<JsonLdService>(serviceProvider, documentService);
            return ActivatorUtilities.CreateInstance<StoredDocumentFacade>(serviceProvider, jsonLdService);
        }
    }
}
