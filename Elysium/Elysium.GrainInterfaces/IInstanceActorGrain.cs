using DotNext;
using Elysium.GrainInterfaces.Services;
using Orleans;
using Orleans.Concurrency;

namespace Elysium.GrainInterfaces
{

    public interface IInstanceActorGrain : IGrainWithGuidKey, IHttpMessageAuthor
    {
        Task<Optional<Exception>> GetDocumentAsync();
        [AlwaysInterleave]
        Task<Optional<Exception>> GenerateDocumentAsync();
    }
}