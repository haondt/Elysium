using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.InstanceActor
{

    public interface IInstanceActorAuthorGrain : IGrainWithGuidKey, IHttpMessageAuthor
    {
        Task<JObject> GenerateDocumentAsync();
        Task<string> GetPublicKeyAsync();
    }
}