using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;

namespace Elysium.GrainInterfaces.LocalActor
{
    public interface ILocalActorAuthorGrain : IGrain<LocalIri>, IHttpMessageAuthor
    {
    }
}
