using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Exceptions;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
using Haondt.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class ActivityPubHttpService(
        IHostingService hostingService,
        IGrainFactory<RemoteUri> uriGrainFactory, 
        IGrainFactory grainFactory, 
        HttpClient httpClient) : IActivityPubHttpService
    {

        public Task VerifySignatureAsync()
        {
            //if (!_hostingService.IsLocalUserUri(data.Target))
            //    return new InvalidOperationException("Cannot publish activities from remote to remote");

            //var lowercaseHeaderDictionary = data.Headers
            //    .ToDictionary(k => k.Key.ToLower(), k => k.Value);

            //if(!lowercaseHeaderDictionary.TryGetValue("signature", out var signatureHeaderValue))
            //    return new InvalidOperationException("missing signature header");

            //var signatureHeaderParts = new Dictionary<string, string>();
            //try
            //{
            //    signatureHeaderParts = signatureHeaderValue.Split(',')
            //        .Select(x => x.Split('='))
            //        .ToDictionary(x => x[0].Trim(), x => x[1].Trim('"'));
            //}
            //catch (Exception ex)
            //{
            //    return ex;
            //}

            //if (!signatureHeaderParts.TryGetValue("keyId", out var keyId))
            //    return new InvalidOperationException("signature header missing keyId");
            //if (!signatureHeaderParts.TryGetValue("signature", out var signature))
            //    return new InvalidOperationException("signature header missing signature");
            //if (!signatureHeaderParts.TryGetValue("headers", out var headerKeys))
            //    return new InvalidOperationException("signature header missing headers");

            //var signatureStringParts = new List<string>();
            //foreach(var headerKey in headerKeys.Split(' '))
            //{
            //    var lowercaseHeaderKey = headerKey.ToLower();
            //    if (lowercaseHeaderKey == "(request-target)")
            //    {
            //        signatureStringParts.Add($"post: {data.}")

            //    }

            //    if (headerKey.ToLower() == "")

            //}

            //try
            //{
            //    var signatureString = string.Join('\n', headers.Split(' ')
            //        .Select(x => $"{x}"lowercaseHeaderDictionary[x.ToLower()]);

            //}
            //catch (Exception ex)
            //{
            //    return ex;
            //}

            //if (!signatureHeaderDictionary.TryGetValue("digest", out var digest))
            //    return new InvalidOperationException("signature header missing digest");
            //if (signatureHeaderDictionary.TryGetValue("date", out var date))
            //{
            //    try
            //    {
            //        var incomingDate = DateTimeOffset.ParseExact(date, "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            //        if (Math.Abs((DateTimeOffset.UtcNow - incomingDate).TotalSeconds) > 30)
            //            return new InvalidOperationException("message date is outside of window");
            //    }
            //    catch (Exception ex)
            //    {
            //        return ex;
            //    }
            //}

            //var signedString = signatureHeaderDictionary

            //var sender = _grainFactory.GetGrain<IRemoteActorGrain>(keyId);
            //var publicKey = await sender.GetPublicKeyAsync();
            //if (!publicKey.IsSuccessful)
            //    return new(publicKey.Error);



            //var recepientGrain = GetLocalGrain(data.Target);
            //if(!recepientGrain.IsSuccessful)
            //    return new(recepientGrain.Error);


            //var payload = JsonSerializer.Deserialize<JObject>(data.Payload);
            //var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);

            //var signatureIsValid = _cryptoService.VerifySignature(data.Digest, data.Payload);

            //await recepientGrain.IngestActivityAsync(new OutgoingRemoteActivityData
            //{
            //    Payload = payload,
            //    Signature = signature,
            //    Date = date,
            //    Digest = digest,
            //});

            //return new();

            throw new NotImplementedException();
        }
        private async Task<Optional<IHostIntegrityGrain>> ValidateHostAsync(RemoteUri target)
        {
            var hostIntegrityGrain = uriGrainFactory.GetGrain<IHostIntegrityGrain>(target);
            if (!await hostIntegrityGrain.ShouldSendRequest())
                return new();
            return new(hostIntegrityGrain);
        }

        public async Task<Result<JToken, ElysiumWebReason>> GetAsync(HttpGetData data)
        {
            var hostIntegrityGrain = await ValidateHostAsync(data.Target);
            if (!hostIntegrityGrain.HasValue)
                return new (ElysiumWebReason.FaultyHost);

            var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            var stringToSign = $"(request-target): get {data.Target.Uri.AbsoluteUri}\nhost: {data.Target.Uri.Host}\ndate: {date}";
            var signature = await data.Author.SignAsync(stringToSign);
            var signatureHeaderValue = $"keyId=\"{await data.Author.GetKeyIdAsync()}\",headers=\"(request-target) host date\",signature=\"{signature}\"";

            var message = new HttpRequestMessage(HttpMethod.Get, data.Target.Uri);
            message.Headers.Add("Host", data.Target.Uri.Host);
            message.Headers.Add("Date", date);
            message.Headers.Add("Signature", signatureHeaderValue);

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(message);
                if (!response.IsSuccessStatusCode)
                {
                    switch(response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            return new(ElysiumWebReason.NotFound);
                    }
                }
            }
            catch
            {
                await hostIntegrityGrain.Value.VoteAgainst();
                throw;
            }
            await hostIntegrityGrain.Value.VoteFor();
            response.EnsureSuccessStatusCode();


            JToken? model;
            try
            {
                model = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                throw new ActivityPubException($"Unable to deserialize data from {data.Target}");
            }
            if (model == null)
                throw new ActivityPubException($"Unable to deserialize data from {data.Target}");

            return new(model);
        }

        public async Task<Result<ElysiumWebReason>> PostAsync(HttpPostData data)
        {
            var hostIntegrityGrain = await ValidateHostAsync(data.Target);
            if (!hostIntegrityGrain.HasValue)
                return new (ElysiumWebReason.FaultyHost);

            var digest = $"SHA-256={SHA256.HashData(Encoding.UTF8.GetBytes(data.JsonLdPayload))}";
            var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            var stringToSign = $"(request-target): post {data.Target.Uri.AbsoluteUri}\nhost: {data.Target.Uri.Host}\ndate: {date}\ndigest: {digest}";
            var signature = await data.Author.SignAsync(stringToSign);
            var signatureHeaderValue = $"keyId=\"{await data.Author.GetKeyIdAsync()}\",headers=\"(request-target) host date digest\",signature=\"{signature}\"";

            var message = new HttpRequestMessage(HttpMethod.Post, data.Target.Uri)
            {
                Content = new StringContent(data.JsonLdPayload, Encoding.UTF8, "application/ld+json"),
            };
            message.Headers.Add("Host", data.Target.Uri.Host);
            message.Headers.Add("Date", date);
            message.Headers.Add("Digest", digest);
            message.Headers.Add("Signature", signatureHeaderValue);

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(message);
                if (!response.IsSuccessStatusCode)
                {
                    switch(response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            return new(ElysiumWebReason.NotFound);
                    }
                }
            }
            catch
            {
                await hostIntegrityGrain.Value.VoteAgainst();
                throw;
            }
            await hostIntegrityGrain.Value.VoteFor();
            response.EnsureSuccessStatusCode();

            return new();
        }
    }
}
