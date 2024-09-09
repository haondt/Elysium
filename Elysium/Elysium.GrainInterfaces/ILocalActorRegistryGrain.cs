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
        public Task<bool> HasRegisteredActor(LocalUri uri);
        public Task RegisterActor(LocalUri uri, LocalActorState initialState);
        public Task UnregisterActor(LocalUri uri);
    }
}
