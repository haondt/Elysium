using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services;

namespace Elysium.GrainInterfaces
{
    public interface ILocalActorAuthorGrain : IGrain<LocalIri>, IHttpMessageAuthor
    {
    }
}
