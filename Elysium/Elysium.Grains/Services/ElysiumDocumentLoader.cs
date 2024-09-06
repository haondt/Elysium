using JsonLD.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class ElysiumDocumentLoader(IDocumentResolver documentResolver) : DocumentLoader
    {
        public override async Task<RemoteDocument> LoadDocumentAsync(string url)
        {
            var document = (await documentResolver.GetDocumentAsync(url)).Value;
            return new RemoteDocument(url, document);
        }
    }
}
