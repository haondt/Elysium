using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IHostIntegrityGrain : IGrainWithStringKey
    {
        public Task VoteAgainst();
        public Task<bool> ShouldSendRequest();
        public Task VoteFor();
    }
}
