using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.Hosting.Services;
using Microsoft.Extensions.Options;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    [StatelessWorker]
    class InstanceActorAuthorGrain(
        IOptions<InstanceActorSettings> options,
        IUserCryptoService cryptoService,
        IIriService iriService) : Grain, IInstanceActorAuthorGrain
    {
        public Task<LocalIri> GetIriAsync() => Task.FromResult(iriService.InstanceActorIri);

        public async Task<string> GetKeyIdAsync() => (await GetIriAsync()).ToString();

        public Task<string> SignAsync(string stringToSign)
        {
            return Task.FromResult(cryptoService.Sign(stringToSign, Convert.FromBase64String(options.Value.PrivateKey)));
        }
    }
}
