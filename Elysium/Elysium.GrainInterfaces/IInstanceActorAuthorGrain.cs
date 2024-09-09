using Elysium.GrainInterfaces.Services;
using Orleans;
using Orleans.Concurrency;

namespace Elysium.GrainInterfaces
{

    public interface IInstanceActorAuthorGrain : IGrainWithGuidKey, IHttpMessageAuthor
    {
    }
}