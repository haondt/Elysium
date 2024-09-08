using DotNext;
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
        private readonly Result<ITypedActorService> _typedActorService;
        private readonly Lazy<Task<Result<byte[]>>> _signingKeyLazy;
        private readonly IUserCryptoService _cryptoService;

        public LocalActorAuthorGrain(
            ITypedActorServiceProvider typedActorServiceProvider,
            IUserCryptoService cryptoService,
            IGrainFactory<LocalUri> grainFactory)
        {
            _id = grainFactory.GetIdentity(this);
            _typedActorService = typedActorServiceProvider.GetService(_id.Uri);
            _signingKeyLazy = new(async () =>
            {
                if (!_typedActorService.IsSuccessful)
                    return new(_typedActorService.Error);
                return await _typedActorService.Value.GetSigningKeyAsync(_id);
            });
            _cryptoService = cryptoService;
        }

        public Task<string> GetKeyIdAsync() => Task.FromResult(_id.Uri.AbsoluteUri);

        public async Task<Result<string>> SignAsync(string stringToSign)
        {
            var signingKey = await _signingKeyLazy.Value;
            if (!signingKey.IsSuccessful)
                return new Result<string>(signingKey.Error);
            return _cryptoService.Sign(stringToSign, signingKey.Value);
        }
    }
}
