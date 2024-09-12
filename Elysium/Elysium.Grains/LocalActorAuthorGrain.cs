using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [StatelessWorker, Reentrant]
    public class LocalActorAuthorGrain : Grain, ILocalActorAuthorGrain
    {
        private readonly LocalIri _id;
        private readonly ILocalActorGrain _mom;
        private readonly IUserCryptoService _cryptoService;
        private byte[]? _signingKey;

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
            _signingKey = await _mom.GetSigningKeyAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public Task<LocalIri> GetIriAsync() => Task.FromResult(_id);

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Iri.ToString());

        public Task<string> SignAsync(string stringToSign)
        {
            return Task.FromResult(_cryptoService.Sign(stringToSign, _signingKey!));
        }

        public Task<bool> IsInASigningMoodAsync() => Task.FromResult(true);
    }
}
