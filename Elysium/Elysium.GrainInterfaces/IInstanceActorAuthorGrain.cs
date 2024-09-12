using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Concurrency;

namespace Elysium.GrainInterfaces
{

    public interface IInstanceActorAuthorGrain : IGrainWithGuidKey, IHttpMessageAuthor
    {
        Task<JObject> GenerateDocumentAsync();
        Task<string> GetPublicKeyAsync();
    }
}