using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.Hosting
{
    public interface IHostIntegrityGrain : IGrain<RemoteIri>
    {
        public Task VoteAgainst();
        public Task<bool> ShouldSendRequest();
        public Task VoteFor();
    }
}
