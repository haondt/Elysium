using Elysium.Core.Models;
using Elysium.GrainInterfaces.Services.GrainFactories;

namespace Elysium.GrainInterfaces.Generics
{
    public class IriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<Iri>
    {
        public TGrain GetGrain<TGrain>(Iri identity) where TGrain : IGrain<Iri>
        {
            return grainFactory.GetGrain<TGrain>(identity.ToString());
        }

        public Iri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<Iri>
        {
            return Iri.FromUnencodedString(grain.GetPrimaryKeyString());
        }
    }
}
