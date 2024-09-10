using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [StatelessWorker]
    public class LocalActorRegistryGrain(IStorageKeyGrainFactory<LocalActorState> grainFactory) : Grain, ILocalActorRegistryGrain
    {
        private IStorageKeyGrain<LocalActorState> GetGrain(LocalIri iri)
        {
            return grainFactory.GetGrain(LocalActorState.CreateStorageKey(iri));
        }
        public Task<bool> HasRegisteredActor(LocalIri iri)
        {
            var grain = GetGrain(iri);
            return grain.ExistsAsync();
        }

        public async Task RegisterActor(LocalIri iri, LocalActorState initialState)
        {
            var grain = GetGrain(iri);
            if (await grain.ExistsAsync())
                throw new InvalidOperationException($"Actor with iri {iri} already exists");
            await grain.SetAsync(initialState);
        }

        public Task UnregisterActor(LocalIri iri)
        {
            var grain = GetGrain(iri);
            return grain.ClearAsync();
        }
    }
}
