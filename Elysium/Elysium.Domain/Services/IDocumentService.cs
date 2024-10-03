using Elysium.Core.Models;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;

namespace Elysium.Domain.Services
{
    public interface IDocumentService
    {
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, Iri iri);
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, RemoteIri iri);
        Task<Result<JToken, ElysiumWebReason>> GetDocumentAsync(IHttpMessageAuthor requester, LocalIri iri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="objectUri"></param>
        /// <param name="document">should NOT contain bto/bcc items</param>
        /// <param name="bto">may be an empty list</param>
        /// <param name="bcc">may be an empty list</param>
        /// <returns></returns>
        Task<Result<DocumentReason>> CreateDocumentAsync(
            LocalIri actor,
            LocalIri objectUri,
            JToken document,
            List<Iri> bto,
            List<Iri> bcc);
        Task<Result<DocumentReason>> ReserveDocumentIriAsync(LocalIri actor, LocalIri documentIri);
        Task<LocalIri> ReserveDocumentIriAsync(LocalIri actor, Func<LocalIri> iriFactory, int maxAttempts);
        Task<Result<JArray, ElysiumWebReason>> GetExpandedDocumentAsync(IHttpMessageAuthor requester, Iri iri);
        Task<Result<JArray, ElysiumWebReason>> GetExpandedDocumentAsync(IHttpMessageAuthor requester, LocalIri iri);
        Task<Result<JArray, ElysiumWebReason>> GetExpandedDocumentAsync(IHttpMessageAuthor requester, RemoteIri iri);
    }
}
