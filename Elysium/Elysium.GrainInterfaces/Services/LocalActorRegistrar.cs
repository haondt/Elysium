using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Haondt.Identity.StorageKey;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class LocalActorRegistrar(IGrainFactory<LocalIri> grainFactory) : ILocalActorRegistrar
    {
        public Task<bool> HasRegisteredActor(LocalIri iri)
        {
            var grain = grainFactory.GetGrain<ILocalActorGrain>(iri);
            return grain.IsInitializedAsync();
        }

        public Task RegisterActor(LocalIri iri, ActorRegistrationDetails registrationDetails)
        {
            var grain = grainFactory.GetGrain<ILocalActorGrain>(iri);
            return grain.InitializeAsync(registrationDetails);
        }

        public Task UnregisterActor(LocalIri iri)
        {
            var grain = grainFactory.GetGrain<ILocalActorGrain>(iri);
            return grain.ClearAsync();
        }
    }
}
