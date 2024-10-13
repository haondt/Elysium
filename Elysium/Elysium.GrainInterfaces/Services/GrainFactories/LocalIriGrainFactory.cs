using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.Services.GrainFactories
{
    public class LocalIriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<LocalIri>
    {
        public TGrain GetGrain<TGrain>(LocalIri identity) where TGrain : IGrain<LocalIri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Iri.ToString());
        }

        public LocalIri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<LocalIri>
        {
            return new LocalIri { Iri = Iri.FromUnencodedString(grain.GetPrimaryKeyString()) };
        }
    }
}
