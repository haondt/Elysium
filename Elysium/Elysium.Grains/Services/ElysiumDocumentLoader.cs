using Elysium.GrainInterfaces.Services;
using JsonLD.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class ElysiumDocumentLoader(
        IDocumentResolver documentResolver,
        IHttpMessageAuthor author, 
        IOptions<HostingSettings> hostingOptions) : DocumentLoader
    {
        public override async Task<RemoteDocument> LoadDocumentAsync(string url)
        {
            var uri = new Uri(url);
            if (uri.Host.Equals(hostingOptions.Value.Host))
            {
                var document = (await documentResolver.GetDocumentAsync(author, new LocalUri { Uri = uri })).Value;
                return new RemoteDocument(url, document);
            }
            else
            {
                var document = (await documentResolver.GetDocumentAsync(author, new RemoteUri { Uri = uri })).Value;
                return new RemoteDocument(url, document);
            }
        }
    }
}
