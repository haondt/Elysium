using DotNext;
using Elysium.Hosting.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface ILocalActorRegistryGrain : IGrainWithGuidKey
    {
        public Task<Result<RegisteredActorState>> GetActor(LocalUri uri);
        public Task<Result<bool>> ExistsActor(LocalUri uri);
        public Task<Optional<Exception>> AddActor(LocalUri uri, RegisteredActorState state);
        public Task<Result<Optional<RegisteredActorState>>> DeleteActor(LocalUri uri);
    }
}
