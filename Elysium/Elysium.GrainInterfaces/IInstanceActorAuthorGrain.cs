using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces
{

    public interface IInstanceActorAuthorGrain : IGrainWithGuidKey, IHttpMessageAuthor
    {
        Task<JObject> GenerateDocumentAsync();
        Task<string> GetPublicKeyAsync();
    }
}