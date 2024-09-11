using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
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
        IDocumentService documentResolver,
        IHttpMessageAuthor author, 
        IHostingService hostingService) : DocumentLoader
    {
        public override async Task<RemoteDocument> LoadDocumentAsync(string url)
        {
            var iri = Iri.FromUnencodedString(url);
            if (hostingService.Host == iri.Host)
            {
                var document = (await documentResolver.GetDocumentAsync(author, new LocalIri { Iri = iri })).Value;
                return new RemoteDocument(url, document);
            }
            else
            {
                var document = (await documentResolver.GetDocumentAsync(author, new RemoteIri { Iri = iri })).Value;
                return new RemoteDocument(url, document);
            }
        }
    }

    //public class ElysiumRemoteDocumentLoader(
    //    IDocumentResolver documentResolver,
    //    IHostingService hostingService) : DocumentLoader
    //{
    //    public override async Task<RemoteDocument> LoadDocumentAsync(string url)
    //    {
    //        var iri = new Iri(url);
    //        if (hostingService.IsLocalHost(iri))
    //        {
    //            var document = (await documentResolver.GetDocumentAsync(author, new RemoteUri { Iri = iri })).Value;
    //            return new RemoteDocument(url, document);
    //        }
    //        else
    //        {
    //            var document = (await documentResolver.GetDocumentAsync(author, new LocalUri { Iri = iri })).Value;
    //            return new RemoteDocument(url, document);
    //        }
    //    }
    //}
}
