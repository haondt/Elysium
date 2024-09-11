using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
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
