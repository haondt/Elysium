using Elysium.Core.Models;
using Elysium.Domain.Exceptions;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Reasons;
using Elysium.GrainInterfaces.Services;
using Haondt.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Elysium.Domain.Services
{
    public class ActivityPubHttpService(
        IOptions<ActivityPubHttpSettings> settings,
        IGrainFactory<RemoteIri> uriGrainFactory,
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
        private async Task<(IHostIntegrityGrain, bool)> ValidateHostAsync(RemoteIri target)
        {
            var hostIntegrityGrain = uriGrainFactory.GetGrain<IHostIntegrityGrain>(target);
            return (hostIntegrityGrain, await hostIntegrityGrain.ShouldSendRequest());
        }

        private async Task<Result<HttpResponseMessage, ElysiumWebReason>> SendAndFollowRedirectsAsync(HttpMethod method, HttpRequestData data, string? jsonLdPayload = null)
        {
            async Task<HttpRequestMessage> buildMessageAsync(HttpMethod method, Uri location, string? jsonLdPayload = null)
            {
                var message = new HttpRequestMessage(method, location);
                if (!string.IsNullOrEmpty(jsonLdPayload))
                    message.Content = new StringContent(jsonLdPayload, Encoding.UTF8, "application/ld+json");

                if (settings.Value.SignFetches && await data.Author.IsInASigningMoodAsync())
                {
                    var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
                    var stringToSign = $"(request-target): {method.ToString().ToLower()} {data.Target.Iri}\nhost: {location.Host}\ndate: {date}";
                    var digestHeaderPart = "";
                    if (!string.IsNullOrEmpty(jsonLdPayload))
                    {
                        var digest = $"SHA-256={SHA256.HashData(Encoding.UTF8.GetBytes(jsonLdPayload))}";
                        stringToSign += $"\ndigest {digest}";
                        message.Headers.Add("Digest", digest);
                        digestHeaderPart = " digest";
                    }
                    var signature = await data.Author.SignAsync(stringToSign);
                    var signatureHeaderValue = $"keyId=\"{await data.Author.GetKeyIdAsync()}\",headers=\"(request-target) host date{digestHeaderPart}\",signature=\"{signature}\"";

                    message.Headers.Add("Host", data.Target.Iri.Host);
                    message.Headers.Add("Date", date);
                    message.Headers.Add("Signature", signatureHeaderValue);
                }

                return message;
            }

            var (hostIntegrityGrain, isValid) = await ValidateHostAsync(data.Target);
            if (!isValid)
                return new(ElysiumWebReason.FaultyHost);

            var redirects = 0;
            HttpResponseMessage? response = null;
            var message = await buildMessageAsync(method, data.Target.Iri, jsonLdPayload);
            do
            {

                //var message = new HttpRequestMessage(HttpMethod.Get, data.Target.Iri);

                try
                {
                    try
                    {
                        response = await httpClient.SendAsync(message);
                        await hostIntegrityGrain.VoteFor();
                    }
                    catch
                    {
                        await hostIntegrityGrain.VoteAgainst();
                        throw;
                    }

                    if (response.IsSuccessStatusCode)
                        return new(response);

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            return new(ElysiumWebReason.NotFound);
                        case System.Net.HttpStatusCode.Redirect:
                        case System.Net.HttpStatusCode.MovedPermanently:
                        case System.Net.HttpStatusCode.TemporaryRedirect:
                        case System.Net.HttpStatusCode.PermanentRedirect:
                            redirects++;
                            if (redirects > settings.Value.MaxRedirects)
                                throw new HttpServiceException(data.Target, data.Author, $"Too many redirects: exceeded {settings.Value.MaxRedirects} redirects while following {data.Target.Iri}.");

                            if (response.Headers.Location == null)
                                throw new InvalidOperationException("Redirect location header is missing.");

                            var newUri = response.Headers.Location.IsAbsoluteUri
                                ? response.Headers.Location
                                : new Uri(message.RequestUri ?? throw new ArgumentNullException(nameof(message.RequestUri)), response.Headers.Location);

                            message = await buildMessageAsync(method, newUri, jsonLdPayload);

                            response.Dispose();
                            continue;
                        default:
                            response.EnsureSuccessStatusCode(); // this will throw an exception
                            throw new HttpRequestException($"Received an unsuccessful status code {response.StatusCode}"); // but just in case :)
                    }

                    // should never happen, but if it somehow does I don't want to spam someone with requests
                    throw new InvalidOperationException($"response code was both successful and unsuccessful: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    response?.Dispose();
                    if (ex is HttpServiceException)
                        throw;
                    throw new HttpServiceException(data.Target, data.Author, null, ex);
                }
            }
            while (true);
        }

        public async Task<Result<JToken, ElysiumWebReason>> GetAsync(HttpGetData data)
        {

            var response = await SendAndFollowRedirectsAsync(HttpMethod.Get, data);

            try
            {

                if (!response.IsSuccessful)
                    return new(response.Reason);

                JToken? model;
                try
                {
                    model = JsonConvert.DeserializeObject<JToken>(await response.Value.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    throw new ActivityPubException($"Unable to deserialize data from {data.Target}", ex);
                }
                if (model == null)
                    throw new ActivityPubException($"Unable to deserialize data from {data.Target}");

                return new(model);
            }
            finally
            {
                if (response.IsSuccessful)
                    response.Value.Dispose();
            }
        }

        public async Task<Result<ElysiumWebReason>> PostAsync(HttpPostData data)
        {
            //var message = new HttpRequestMessage(HttpMethod.Post, data.Target.Iri)
            //{
            //    Content = new StringContent(data.JsonLdPayload, Encoding.UTF8, "application/ld+json"),
            //};

            //if (settings.Value.SignPushes && await data.Author.IsInASigningMoodAsync())
            //{
            //    var digest = $"SHA-256={SHA256.HashData(Encoding.UTF8.GetBytes(data.JsonLdPayload))}";
            //    var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            //    var stringToSign = $"(request-target): post {data.Target.Iri.ToString()}\nhost: {data.Target.Iri.Host}\ndate: {date}\ndigest: {digest}";
            //    var signature = await data.Author.SignAsync(stringToSign);
            //    var signatureHeaderValue = $"keyId=\"{await data.Author.GetKeyIdAsync()}\",headers=\"(request-target) host date digest\",signature=\"{signature}\"";

            //    message.Headers.Add("Host", data.Target.Iri.Host);
            //    message.Headers.Add("Date", date);
            //    message.Headers.Add("Digest", digest);
            //    message.Headers.Add("Signature", signatureHeaderValue);
            //}

            var response = await SendAndFollowRedirectsAsync(HttpMethod.Post, data, data.JsonLdPayload);
            try
            {
                if (!response.IsSuccessful)
                    return new(response.Reason);

                return new();
            }
            finally
            {
                if (response.IsSuccessful)
                    response.Value.Dispose();
            }
        }
    }
}
