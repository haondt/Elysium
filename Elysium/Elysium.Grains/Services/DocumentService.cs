using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Server.Services;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haondt.Persistence.Services;
using Elysium.Core.Models;

namespace Elysium.Grains.Services
{
    public class DocumentService(
        IGrainFactory<RemoteIri> remoteGrainFactory, 
        //IGrainFactory<StorageKey<DocumentState>> localGrainFactory, 
        IStorage storage,
        IHostingService hostingService) : IDocumentService
    {
        //private Task<bool> VerifyReadPermissions()
        //{

        //}

        //public Task<Result<JObject>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteUri iri)
        //{
        //    var remoteDocumentGrain = grainFactory.GetGrain<IRemoteDocumentGrain>(iri.ToString());
        //    return remoteDocumentGrain.GetValueAsync(requester);
        //}

        //public Task<Result<JObject>> GetDocumentAsync(Iri requester, LocalUri iri)
        //{
        //    var localDocumentGrain = grainFactory.GetGrain<ILocalDocumentGrain>(iri);
        //    return localDocumentGrain.GetValueAsync(requester);
        //}
        public async Task<Result<DocumentReason>> CreateDocumentAsync(
            LocalIri actor, 
            LocalIri objectUri, 
            JObject compactedObject,
            List<Iri> bto,
            List<Iri> bcc)
        {
            //var documentGrain = localGrainFactory.GetGrain<ILocalDocumentGrain>(objectUri);
            //if (await documentGrain.HasValueAsync())
            //    return new(DocumentReason.Conflict);
            //documentGrain.SetValueAsync

            if (!hostingService.IsScopedToLocalUser(objectUri, actor))
                return new(DocumentReason.Unauthorized);

            var storageKey = DocumentState.GetStorageKey(objectUri);
            if (await storage.ContainsKey(storageKey))
                return new(DocumentReason.Conflict);

            var state = new DocumentState
            {
                Owner = actor,
                Value = compactedObject,
                UpdatedOnUtc = DateTime.UtcNow,
                BCc = bcc,
                BTo = bto
            };

            await storage.Set(storageKey, state);
            return new();
        }

        public Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteIri iri)
        {
            throw new NotImplementedException();
        }

        public Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, LocalIri iri)
        {
            throw new NotImplementedException();
        }

    }
}
