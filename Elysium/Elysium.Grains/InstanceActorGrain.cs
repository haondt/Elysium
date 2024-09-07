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
    class InstanceActorGrain : Grain, IInstanceActorGrain
    {
        public Task<Optional<Exception>> GenerateDocumentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Optional<Exception>> GetDocumentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
