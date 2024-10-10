using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.Services;
using Orleans.Concurrency;

namespace Elysium.Grains.LocalActor
{
    [StatelessWorker, Reentrant]
    public class LocalActorAuthorGrain : Grain, ILocalActorAuthorGrain
    {
        private readonly LocalIri _id;
        private readonly ILocalActorGrain _mom;
        private readonly IUserCryptoService _cryptoService;
        private PlaintextCryptographicActorData? _cryptographicActorData;

        public LocalActorAuthorGrain(
            IUserCryptoService cryptoService,
            IGrainFactory<LocalIri> grainFactory)
        {
            _id = grainFactory.GetIdentity(this);
            _mom = grainFactory.GetGrain<ILocalActorGrain>(_id);
            _cryptoService = cryptoService;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _cryptographicActorData = await _mom.GetCryptographicDataAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public Task<LocalIri> GetIriAsync() => Task.FromResult(_id);

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Iri.ToString());

        public Task<string> SignAsync(string stringToSign)
        {
            return Task.FromResult(_cryptoService.Sign(stringToSign, _cryptographicActorData!.SigningKey));
        }

        public Task<bool> IsInASigningMoodAsync() => Task.FromResult(true);
    }
}
