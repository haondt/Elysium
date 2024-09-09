using Elysium.GrainInterfaces;
using Elysium.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class LocalActorRegistryGrain : Grain, ILocalActorRegistryGrain
    {
        public Task<bool> HasRegisteredActor(LocalUri uri)
        {
            throw new NotImplementedException();
        }

        public Task RegisterActor(LocalUri uri, LocalActorState initialState)
        {
            throw new NotImplementedException();
        }

        public Task UnregisterActor(LocalUri uri)
        {
            throw new NotImplementedException();
        }
    }
}
