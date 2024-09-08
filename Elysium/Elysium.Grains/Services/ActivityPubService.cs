using DotNext;
using Elysium.Authentication.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class ActivityPubService : IActivityPubService
    {
        private readonly IUriGrainFactory _grainFactory;
        private readonly IHostingService _hostingService;
        private readonly IUserCryptoService _cryptoService;

        public ActivityPubService(
            IUriGrainFactory grainFactory,
            IHostingService hostingService, 
            IUserCryptoService cryptoService)
        {
            _grainFactory = grainFactory;
            _hostingService = hostingService;
            _cryptoService = cryptoService;
        }
        public  Task<Optional<Exception>> PublishLocalActivityAsync(LocalUri sender, LocalUri recepient, JObject activity)
        {
            if (sender == recepient)
                return Task.FromResult(Optional<Exception>.None);
            var recepientGrain = _grainFactory.GetGrain<ILocalActorGrain>(recepient);
            return recepientGrain.IngestActivityAsync(activity);
        }
        public async Task<Optional<Exception>> PublishRemoteActivityAsync(LocalUri sender, RemoteUri recepient, JObject activity)
        {
            var senderGrain = _grainFactory.GetGrain<ILocalActorGrain>(sender);
            var signingKey = await senderGrain.GetSigningKeyAsync();
            if(!signingKey.IsSuccessful)
                return new(signingKey.Error);

            var recepientGrain = _grainFactory.GetGrain<IRemoteActorWorkerGrain>(recepient);
            await recepientGrain.IngestActivityAsync(new OutgoingRemoteActivityData
            {
                Payload = JsonConvert.SerializeObject(activity),
                Sender = sender
            });
            return new();
        }
        public async Task<Optional<Exception>> IngestRemoteActivityAsync(IncomingRemoteActivityData data)
        {
            throw new NotImplementedException();
            //var recepientGrain = _grainFactory.GetGrain<IRemoteActorWorkerGrain>(recepient);
            //await recepientGrain.PublishActivityAsync(new OutgoingRemoteActivityData
            //{
            //    Payload = JsonConvert.SerializeObject(activity),
            //    Sender = sender
            //});
        }

    }
}
