using DotNext;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class DocumentResolver(IGrainFactory<RemoteUri> grainFactory, IHostingService hostingService) : IDocumentResolver
    {
        //private Task<bool> VerifyReadPermissions()
        //{

        //}

        //public Task<Result<JObject>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteUri uri)
        //{
        //    var remoteDocumentGrain = grainFactory.GetGrain<IRemoteDocumentGrain>(uri.ToString());
        //    return remoteDocumentGrain.GetValueAsync(requester);
        //}

        //public Task<Result<JObject>> GetDocumentAsync(Uri requester, LocalUri uri)
        //{
        //    var localDocumentGrain = grainFactory.GetGrain<ILocalDocumentGrain>(uri);
        //    return localDocumentGrain.GetValueAsync(requester);
        //}

        public Task<Result<JObject>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteUri uri)
        {
            throw new NotImplementedException();
        }

        public Task<Result<JObject>> GetDocumentAsync(IHttpMessageAuthor requester, LocalUri uri)
        {
            throw new NotImplementedException();
        }
    }
}
