using Elysium.Core.Models;

namespace Elysium.GrainInterfaces.Services.GrainFactories
{
    public class RemoteIriGrainFactory(IGrainFactory grainFactory) : IGrainFactory<RemoteIri>
    {
        public TGrain GetGrain<TGrain>(RemoteIri identity) where TGrain : IGrain<RemoteIri>
        {
            return grainFactory.GetGrain<TGrain>(identity.Iri.ToString());
        }

        public RemoteIri GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<RemoteIri>
        {
            return new RemoteIri { Iri = Iri.FromUnencodedString(grain.GetPrimaryKeyString()) };
        }
    }
}
