using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class LocalActorRegistryGrain : Grain, ILocalActorRegistryGrain
    {
        public Task<bool> HasRegisteredActor(LocalIri iri)
        {
            throw new NotImplementedException();
        }

        public Task RegisterActor(LocalIri iri, LocalActorState initialState)
        {
            throw new NotImplementedException();
        }

        public Task UnregisterActor(LocalIri iri)
        {
            throw new NotImplementedException();
        }
    }
}
