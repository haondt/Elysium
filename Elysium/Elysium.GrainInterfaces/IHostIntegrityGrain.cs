using Elysium.Hosting.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IHostIntegrityGrain : IGrain<RemoteIri>
    {
        public Task VoteAgainst();
        public Task<bool> ShouldSendRequest();
        public Task VoteFor();
    }
}
