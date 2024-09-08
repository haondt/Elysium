using DotNext;
using Elysium.GrainInterfaces;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [StatelessWorker]
    class InstanceActorGrain : Grain, IInstanceActorAuthorGrain
    {
        public Task<string> GetKeyIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> SignAsync(string stringToSign)
        {
            throw new NotImplementedException();
        }
    }
}
