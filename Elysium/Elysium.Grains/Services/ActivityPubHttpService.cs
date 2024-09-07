using DotNext;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Exceptions;
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
        IOptions<HostingSettings> hostingOptions,
        IUriGrainFactory uriGrainFactory, 
        IGrainFactory grainFactory, 
        HttpClient httpClient) : IActivityPubHttpService
    {
        private readonly HostingSettings _hostingSettings = hostingOptions.Value;

        private async Task<Result<IHostIntegrityGrain>> ValidateHostAsync(Uri target)
        {
            if (target.Host ==_hostingSettings.Host)
                return new(new InvalidOperationException("cannot send requests to a local target"));

            var hostIntegrityGrain = grainFactory.GetGrain<IHostIntegrityGrain>(target.Host);
            if (!await hostIntegrityGrain.ShouldSendRequest())
                return new(new ActivityPubException($"The host {target.Host} is not in good standing"));
            return new(hostIntegrityGrain);
        }

        public async Task<Result<JObject>> GetAsync(HttpGetData data)
        {
            var hostIntegrityGrain = await ValidateHostAsync(data.Target);
            if (!hostIntegrityGrain.IsSuccessful)
                return new(hostIntegrityGrain.Error);

            var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            var stringToSign = $"(request-target): get {data.Target.AbsoluteUri}\nhost: {data.Target.Host}\ndate: {date}";
            var signature = await data.Author.SignAsync(stringToSign);
            var signatureHeaderValue = $"keyId=\"{await data.Author.GetKeyIdAsync()}\",headers=\"(request-target) host date\",signature=\"{signature}\"";

            var message = new HttpRequestMessage(HttpMethod.Get, data.Target);
            message.Headers.Add("Host", data.Target.Host);
            message.Headers.Add("Date", date);
            message.Headers.Add("Signature", signatureHeaderValue);

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(message);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await hostIntegrityGrain.Value.VoteAgainst();
                return new(ex);
            }

            await hostIntegrityGrain.Value.VoteFor();

            JObject? model;
            try
            {
                model = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                if (model == null)
                    return new(new ActivityPubException($"Unable to deserialize data from {data.Target}"));
            }
            catch
            {
                return new(new ActivityPubException($"Unable to deserialize data from {data.Target}"));
            }

            return new(model);
        }

        public async Task<Optional<Exception>> PostAsync(HttpPostData data)
        {
            var hostIntegrityGrain = await ValidateHostAsync(data.Target);
            if (!hostIntegrityGrain.IsSuccessful)
                return new(hostIntegrityGrain.Error);

            var digest = $"SHA-256={SHA256.HashData(Encoding.UTF8.GetBytes(data.JsonLdPayload))}";
            var date = DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            var stringToSign = $"(request-target): post {data.Target.AbsoluteUri}\nhost: {data.Target.Host}\ndate: {date}\ndigest: {digest}";
            var signature = await data.Author.SignAsync(stringToSign);
            var signatureHeaderValue = $"keyId=\"{await data.Author.GetKeyIdAsync()}\",headers=\"(request-target) host date digest\",signature=\"{signature}\"";

            var message = new HttpRequestMessage(HttpMethod.Post, data.Target)
            {
                Content = new StringContent(data.JsonLdPayload, Encoding.UTF8, "application/ld+json"),
            };
            message.Headers.Add("Host", data.Target.Host);
            message.Headers.Add("Date", date);
            message.Headers.Add("Digest", digest);
            message.Headers.Add("Signature", signatureHeaderValue);

            try
            {
                var response = await httpClient.SendAsync(message);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await hostIntegrityGrain.Value.VoteAgainst();
                return new(ex);
            }

            await hostIntegrityGrain.Value.VoteFor();
            return new();
        }
    }
}
