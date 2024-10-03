using Elysium.Core.Models;
using Elysium.Domain.Exceptions;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Haondt.Core.Models;
using Haondt.Persistence.Services;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain.Services
{
    public class DocumentService : IDocumentService

    {
        private readonly IInstanceActorAuthorGrain _instanceActorGrain;
        private readonly IStoredDocumentFacade _documentFacade;
        private readonly IActivityPubHttpService _httpService;
        private readonly IHostingService _hostingService;
        private readonly IIriService _iriService;
        public DocumentService(
        //IGrainFactory<StorageKey<DocumentState>> localGrainFactory, 
        IActivityPubHttpService httpService,
        IGrainFactory baseGrainFactory,
        IHostingService hostingService,
        IStoredDocumentFacadeFactory documentFacadeFactory,
        IIriService iriService)
        {
            _instanceActorGrain = baseGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);
            _documentFacade = documentFacadeFactory.Create(this);
            _httpService = httpService;
            _hostingService = hostingService;
            _iriService = iriService;
        }

        //private Task<bool> VerifyReadPermissions()
        //{

        //}

        //public Task<Result<JObject>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteUri iri)
        //{
        //}

        //public Task<Result<JObject>> GetDocumentAsync(Iri requester, LocalUri iri)
        //{
        //    var localDocumentGrain = grainFactory.GetGrain<ILocalDocumentGrain>(iri);
        //    return localDocumentGrain.GetValueAsync(requester);
        //}
        public async Task<Result<DocumentReason>> CreateDocumentAsync(
            LocalIri actor,
            LocalIri documentIri,
            JToken document, // does not contain bto/bcc items
            List<Iri> bto,
            List<Iri> bcc)
        {
            ArgumentNullException.ThrowIfNull(document);
            //var documentGrain = localGrainFactory.GetGrain<ILocalDocumentGrain>(objectUri);
            //if (await documentGrain.HasValueAsync())
            //    return new(DocumentReason.Conflict);
            //documentGrain.SetValueAsync

            if (_iriService.IsScopedToLocalActor(documentIri))
                if (!_iriService.IsScopedToLocalActor(documentIri, actor))
                    return new(DocumentReason.Unauthorized);

            //var storageKey = LocalDocumentState.GetStorageKey(objectUri);
            //if (await storage.ContainsKey(storageKey))
            //var storageKeyGrain = localDocumentGrainFactory.GetGrain(DocumentState.GetStorageKey(documentIri.Iri));
            var existingState = await _documentFacade.GetAsync(documentIri.Iri);
            if (existingState.IsSuccessful)
            {
                if (existingState.Value.IsReservation)
                {
                    if (existingState.Value.Owner != actor)
                        return new(DocumentReason.Conflict);
                }
                else
                    return new(DocumentReason.Conflict);
            }

            var state = new DocumentState
            {
                Owner = actor,
                Value = document,
                UpdatedOnUtc = DateTime.UtcNow,
                BCc = bcc,
                BTo = bto,
                IsReservation = false
            };

            //await storage.Set(storageKey, state);
            await _documentFacade.SetAsync(documentIri.Iri, state);
            //await storageKeyGrain.SetAsync(state);
            return new();
        }

        public async Task<LocalIri> ReserveDocumentIriAsync(LocalIri actor, Func<LocalIri> iriFactory, int maxAttempts)
        {
            var currentIriGenerationAttempt = 1;
            while (currentIriGenerationAttempt < maxAttempts)
            {
                var iri = iriFactory();
                var objectIdReserved = await ReserveDocumentIriAsync(actor, iri);
                if (objectIdReserved.IsSuccessful)
                    return iri;

                currentIriGenerationAttempt++;
            }
            throw new ProbabilityException($"couldn't generate a unique iri after {maxAttempts} attempts");

        }
        public async Task<Result<DocumentReason>> ReserveDocumentIriAsync(LocalIri actor, LocalIri documentIri)
        {
            //var storageKeyGrain = localDocumentGrainFactory.GetGrain(DocumentState.GetStorageKey(documentIri));
            //if (await storageKeyGrain.ExistsAsync())
            if (await _documentFacade.ExistsAsync(documentIri.Iri))
                return new(DocumentReason.Conflict);
            await _documentFacade.SetAsync(documentIri.Iri, new DocumentState
            {
                IsReservation = true,
                Owner = actor
            });

            return new();
        }


        public Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, Iri iri)
        {
            if (iri.Host == _hostingService.Host)
                return GetDocumentAsync(requester, new LocalIri { Iri = iri });
            return GetDocumentAsync(requester, new RemoteIri { Iri = iri });

        }

        public async Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteIri iri)
        {
            // todo: permission checks on the result, since it may be cached from a different requester
            // also todo, limiting how long the remote document cache lasts for
            var document = await _documentFacade.GetAsync(iri.Iri);
            if (document.IsSuccessful)
            {
                if (document.Value.Value == null)
                    throw new NullReferenceException($"remote iri {iri} yielded a null object");
                return new(document.Value.Value);
            }
            else if (document.Reason != StorageResultReason.NotFound)
                return new(MapReason(document.Reason));

            // pull the document
            var httpResult = await _httpService.GetAsync(new HttpGetData
            {
                Author = requester,
                Target = iri
            });

            if (!httpResult.IsSuccessful)
                return httpResult;

            await _documentFacade.SetAsync(iri.Iri, new DocumentState
            {
                Owner = await requester.GetIriAsync(),
                UpdatedOnUtc = DateTime.UtcNow,
                Value = httpResult.Value
            });

            return httpResult;
        }

        private ElysiumWebReason MapReason(StorageResultReason reason) => reason switch
        {

            StorageResultReason.NotFound => ElysiumWebReason.NotFound,
            _ => throw new InvalidCastException($"Unable to map {typeof(StorageResultReason)}.{reason} to {typeof(ElysiumWebReason)}")
        };

        public Task<Result<JArray, ElysiumWebReason>> GetExpandedDocumentAsync(IHttpMessageAuthor requester, Iri iri)
        {

            if (iri.Host == _hostingService.Host)
                return GetExpandedDocumentAsync(requester, new LocalIri { Iri = iri });
            return GetExpandedDocumentAsync(requester, new RemoteIri { Iri = iri });
        }

        public async Task<Result<JArray, ElysiumWebReason>> GetExpandedDocumentAsync(IHttpMessageAuthor requester, RemoteIri iri)
        {
            var document = await _documentFacade.GetExpandedAsync(iri.Iri);
            if (!document.IsSuccessful)
                return new(MapReason(document.Reason));
            return new(document.Value.Value ?? throw new NullReferenceException($"document with id {iri} exists but has a null value"));
        }

        public Task<Result<JArray, ElysiumWebReason>> GetExpandedDocumentAsync(IHttpMessageAuthor requester, LocalIri iri)
        {
            return GetDocumentAsync<JArray>(requester, iri, async (iri) =>
            {
                var result = await _documentFacade.GetExpandedAsync(iri);
                return result.IsSuccessful
                    ? new(result.Value)
                    : new(result.Reason);
            });
        }

        public Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, LocalIri iri)
        {
            return GetDocumentAsync<JToken>(requester, iri, async (iri) =>
            {
                var result = await _documentFacade.GetAsync(iri);
                return result.IsSuccessful
                    ? new(result.Value)
                    : new(result.Reason);
            });
        }

        private async Task<Result<T, ElysiumWebReason>> GetDocumentAsync<T>(
            IHttpMessageAuthor requester,
            LocalIri iri,
            Func<Iri, Task<Result<DocumentState<T>, StorageResultReason>>> fetcher) where T : JToken
        {
            // todo: permission checks on the result, since the requester may or may not be in the to/bto/bcc/cc/audience/author
            // todo: populate the bto/bcc based on the requesters identity
            // this would mean using the document facade to expand the document, then setting the info, then (optionally) compacting it back down

            if (iri.Iri == _iriService.InstanceActorIri.Iri)
            {
                var documentResult = await fetcher(_iriService.InstanceActorIri.Iri);
                if (documentResult.IsSuccessful)
                    return new(documentResult.Value.Value!);
                var instanceActorDocument = await _instanceActorGrain.GenerateDocumentAsync();
                await _documentFacade.SetAsync(_iriService.InstanceActorIri.Iri, new DocumentState
                {
                    Value = instanceActorDocument
                });

                if (instanceActorDocument is T casted)
                    return new(casted);
                var converted = await fetcher(_iriService.InstanceActorIri.Iri);
                if (converted.IsSuccessful)
                    return new(converted.Value.Value ?? throw new NullReferenceException($"document with id {iri} exists but has a null value"));
                return new(MapReason(converted.Reason));
            }

            var document = await fetcher(iri.Iri);
            if (!document.IsSuccessful)
                return new(MapReason(document.Reason));

            // uninitialized document
            if (document.Value.Owner == null)
                return new(ElysiumWebReason.NotFound);

            // reserved but not yet populated
            if (document.Value.IsReservation)
                return new(ElysiumWebReason.NotFound);


            return new(document.Value.Value ?? throw new NullReferenceException($"document with id {iri} exists but has a null value"));
        }
    }
}
