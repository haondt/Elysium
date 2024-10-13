namespace Elysium.GrainInterfaces.Services.GrainFactories
{
    public interface IGrainFactory<TIdentity>
    {
        TGrain GetGrain<TGrain>(TIdentity identity) where TGrain : IGrain<TIdentity>;
        TIdentity GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<TIdentity>;
    }
}
