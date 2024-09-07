using DotNext;
using Elysium.Authentication.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Microsoft.Extensions.Options;
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
        private readonly HostingSettings _hostingSettings;
        private readonly IHostingService _hostingService;
        private readonly IUserCryptoService _cryptoService;

        public ActivityPubService(IUriGrainFactory grainFactory, IHostingService hostingService, 
            IOptions<HostingSettings> hostingOptions, IUserCryptoService cryptoService)
        {
            _grainFactory = grainFactory;
            _hostingSettings = hostingOptions.Value;
            _hostingService = hostingService;
            _cryptoService = cryptoService;
        }
        public Task<Optional<Exception>> PublishLocalActivityAsync(Uri sender, Uri recepient, JObject activity)
        {
            if (!sender.Host.Equals(_hostingSettings.Host))
                return Task.FromResult(new Optional<Exception>(new InvalidOperationException("sender must be a local actor")));
            if (sender == recepient)
                return Task.FromResult(Optional<Exception>.None);
            if (!recepient.Host.Equals(_hostingSettings.Host))
                return SendLocalActivityToLocalGrain(recepient, activity);
            return SendLocalActivityToRemoteGrain(sender, recepient, activity);
        }


        private async Task<Optional<Exception>> SendLocalActivityToLocalGrain(Uri recepient, JObject activity)
        {
            var recepientGrain = _grainFactory.GetGrain<ILocalActorGrain>(recepient);
            return await recepientGrain.IngestActivityAsync(activity);
        }

        private async Task<Optional<Exception>> SendLocalActivityToRemoteGrain(Uri sender, Uri recepient, JObject activity)
        {
            var senderGrain = _grainFactory.GetGrain<ILocalActorGrain>(sender);
            var signingKey = await senderGrain.GetSigningKeyAsync();
            if(!signingKey.IsSuccessful)
                return new(signingKey.Error);

            var recepientGrain = _grainFactory.GetGrain<IRemoteActorWorkerGrain>(recepient);

            return new();
        }
        public async Task<Optional<Exception>> IngestRemoteActivityAsync(OutgoingRemoteActivityData data)
        {
            // TODO: validate acitivty (required fields, etc)
            if (!_hostingService.IsLocalUserUri(data.Target))
                return new InvalidOperationException("Cannot publish activities from remote to remote");

            var lowercaseHeaderDictionary = data.Headers
                .ToDictionary(k => k.Key.ToLower(), k => k.Value);

            if(!lowercaseHeaderDictionary.TryGetValue("signature", out var signatureHeaderValue))
                return new InvalidOperationException("missing signature header");

            var signatureHeaderParts = new Dictionary<string, string>();
            try
            {
                signatureHeaderParts = signatureHeaderValue.Split(',')
                    .Select(x => x.Split('='))
                    .ToDictionary(x => x[0].Trim(), x => x[1].Trim('"'));
            }
            catch (Exception ex)
            {
                return ex;
            }

            if (!signatureHeaderParts.TryGetValue("keyId", out var keyId))
                return new InvalidOperationException("signature header missing keyId");
            if (!signatureHeaderParts.TryGetValue("signature", out var signature))
                return new InvalidOperationException("signature header missing signature");
            if (!signatureHeaderParts.TryGetValue("headers", out var headerKeys))
                return new InvalidOperationException("signature header missing headers");

            var signatureStringParts = new List<string>();
            foreach(var headerKey in headerKeys.Split(' '))
            {
                var lowercaseHeaderKey = headerKey.ToLower();
                if (lowercaseHeaderKey == "(request-target)")
                {
                    signatureStringParts.Add($"post: {data.}")

                }

                if (headerKey.ToLower() == "")

            }

            try
            {
                var signatureString = string.Join('\n', headers.Split(' ')
                    .Select(x => $"{x}"lowercaseHeaderDictionary[x.ToLower()]);
                    
            }
            catch (Exception ex)
            {
                return ex;
            }

            if (!signatureHeaderDictionary.TryGetValue("digest", out var digest))
                return new InvalidOperationException("signature header missing digest");
            if (signatureHeaderDictionary.TryGetValue("date", out var date))
            {
                try
                {
                    var incomingDate = DateTimeOffset.ParseExact(date, "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    if (Math.Abs((DateTimeOffset.UtcNow - incomingDate).TotalSeconds) > 30)
                        return new InvalidOperationException("message date is outside of window");
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            var signedString = signatureHeaderDictionary

            var sender = _grainFactory.GetGrain<IRemoteActorGrain>(keyId);
            var publicKey = await sender.GetPublicKeyAsync();
            if (!publicKey.IsSuccessful)
                return new(publicKey.Error);



            var recepientGrain = GetLocalGrain(data.Target);
            if(!recepientGrain.IsSuccessful)
                return new(recepientGrain.Error);


            var payload = JsonSerializer.Deserialize<JObject>(data.Payload);
            var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);

            var signatureIsValid = _cryptoService.VerifySignature(data.Digest, data.Payload);

            await recepientGrain.IngestActivityAsync(new OutgoingRemoteActivityData
            {
                Payload = payload,
                Signature = signature,
                Date = date,
                Digest = digest,
            });

            return new();
        }
    }
}
