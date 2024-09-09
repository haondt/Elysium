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
        public Task<bool> HasRegisteredActor(LocalIri uri);
        public Task RegisterActor(LocalIri uri, LocalActorState initialState);
        public Task UnregisterActor(LocalIri uri);
    }
}
