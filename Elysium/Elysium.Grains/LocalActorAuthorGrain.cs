using Elysium.Authentication.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [StatelessWorker]
    public class LocalActorAuthorGrain : Grain, ILocalActorAuthorGrain
    {
        private readonly LocalUri _id;
        private readonly Lazy<Task<byte[]>> _signingKeyLazy;
        private readonly IUserCryptoService _cryptoService;

        public LocalActorAuthorGrain(
            ITypedActorServiceProvider typedActorServiceProvider,
            IUserCryptoService cryptoService,
            IGrainFactory<LocalUri> grainFactory)
        {
            _id = grainFactory.GetIdentity(this);
            var typedActorService = typedActorServiceProvider.GetService(_id.Uri);
            _signingKeyLazy = new(typedActorService.GetSigningKeyAsync(_id));
            _cryptoService = cryptoService;
        }

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Uri.AbsoluteUri);

        public async Task<string> SignAsync(string stringToSign)
        {
            var signingKey = await _signingKeyLazy.Value;
            return _cryptoService.Sign(stringToSign, signingKey);
        }
    }
}
