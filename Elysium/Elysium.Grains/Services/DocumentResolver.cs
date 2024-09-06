using DotNext;
using Elysium.GrainInterfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class DocumentResolver(IGrainFactory grainFactory, IHostingService hostingService) : IDocumentResolver
    {
        public Task<Result<JObject>> GetDocument(Uri uri)
        {
            if(hostingService.IsLocalUserUri(uri))
            {
                var localDocumentGrain = grainFactory.GetGrain<ILocalDocumentGrain>(uri.ToString());
                return localDocumentGrain.GetValueAsync();
            }
            var remoteDocumentGrain = grainFactory.GetGrain<IRemoteDocumentGrain>(uri.ToString());
            return remoteDocumentGrain.GetValueAsync();
        }

        public Task<Result<JObject>> GetDocumentAsync(string url) => GetDocument(new Uri(url));
    }
}
