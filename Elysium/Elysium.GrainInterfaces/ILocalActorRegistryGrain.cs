using Elysium.Core.Models;
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
        /// <summary>
        /// Registers a local actor so that they may receive activities
        /// </summary>
        /// <param name="iri"></param>
        /// <returns></returns>
        public Task<bool> HasRegisteredActor(LocalIri iri);
        public Task RegisterActor(LocalIri iri, LocalActorState initialState);
        public Task UnregisterActor(LocalIri iri);
    }
}
