using DotNext;
using Elysium.Authentication.Services;
using Elysium.GrainInterfaces;
using KristofferStrube.ActivityStreams;
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
        private readonly IGrainFactory _grainFactory;
        private readonly IHostingService _hostingService;
        private readonly IUserCryptoService _cryptoService;

        public ActivityPubService(IGrainFactory grainFactory, IHostingService hostingService, IUserCryptoService cryptoService)
        {
            _grainFactory = grainFactory;
            _hostingService = hostingService;
            _cryptoService = cryptoService;
        }
        public Task<Optional<Exception>> PublishActivityAsync(Uri sender, Uri recepient, JObject activity)
        {
            if (sender == recepient)
                return Task.FromResult(Optional<Exception>.None);
            if (_hostingService.IsLocalUserUri(recepient))
                return SendActivityToLocalGrain(recepient, activity);
            if (!_hostingService.IsLocalUserUri(sender))
                return Task.FromResult<Optional<Exception>>(new InvalidOperationException("Cannot publish activities from remote to remote"));
            return SendActivityToRemoteGrain(sender, recepient, activity);
        }

        private Result<ILocalActorGrain> GetLocalGrain(Uri id)
        {
            var username = _hostingService.GetLocalUserFromUri(id);
            if (!username.IsSuccessful)
                return new(username.Error);
            return new(_grainFactory.GetGrain<ILocalActorGrain>(username.Value));
        }

        private async Task<Optional<Exception>> SendActivityToLocalGrain(Uri recepient, JObject activity)
        {
            var recepientGrain = GetLocalGrain(recepient);
            if(!recepientGrain.IsSuccessful)
                return new(recepientGrain.Error);

            return await recepientGrain.Value.IngestActivityAsync(activity);
        }
        private async Task<Optional<Exception>> SendActivityToRemoteGrain(Uri sender, Uri recepient, JObject activity)
        {
            var senderGrain = GetLocalGrain(sender);
            if(!senderGrain.IsSuccessful)
                return new(senderGrain.Error);
            var signingKey = await senderGrain.Value.GetSigningKey();
            if(!signingKey.IsSuccessful)
                return new(signingKey.Error);

            var recepientGrain = _grainFactory.GetGrain<IRemoteActorGrain>(recepient.ToString());
            var recepientInbox = await recepientGrain.GetInboxUriAsync();
            if (!recepientInbox.IsSuccessful)
                return new(recepientInbox.Error);

            var payload = JsonSerializer.Serialize(activity);
            var digest = $"SHA-256={SHA256.HashData(Encoding.UTF8.GetBytes(payload))}";
            var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            var stringToSign = $"(request-target): post {recepientInbox.Value.AbsolutePath}\nhost: {recepientInbox.Value.Host}\ndate: {date}\ndigest: {digest}";
            var signature = _cryptoService.Sign(stringToSign, signingKey.Value);
            var signatureHeader = $"keyId=\"{sender}\",headers=\"(request-target) host date digest\",signature=\"{signature}\"";

            await recepientGrain.IngestActivityAsync(new OutgoingRemoteActivityData
            {
                Payload = payload,
                Target = recepientInbox.Value,
                CompliantRequestTarget = $"post: {recepientInbox.Value.AbsolutePath}",
                Headers =
                [
                    ( "Host", recepientInbox.Value.Host ),
                    ( "Date", date ),
                    ( "Digest", digest ),
                    ( "Signature", signature )
                ]
            });

            return new();
        }
        public async Task<Optional<Exception>> IngestRemoteActivityAsync(IncomingRemoteActivityData data)
        {
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
