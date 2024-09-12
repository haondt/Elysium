using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Grains.Services;
using Elysium.Hosting.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
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
        IJsonLdService jsonLdService,
        IUserCryptoService cryptoService,
        IIriService iriService) : Grain, IInstanceActorAuthorGrain
    {
        public Task<LocalIri> GetIriAsync() => Task.FromResult(iriService.InstanceActorIri);

        public async Task<string> GetKeyIdAsync() => (await GetIriAsync()).ToString();
        public Task<string> GetPublicKeyAsync() => Task.FromResult(options.Value.PublicKey);

        public Task<string> SignAsync(string stringToSign)
        {
            return Task.FromResult(cryptoService.Sign(stringToSign, Convert.FromBase64String(options.Value.PrivateKey)));
        }
        public async Task<JObject> GenerateDocumentAsync()
        {
            var publicKeyPem = cryptoService.EncodePublicKeyToPemX509(options.Value.PublicKey);
            var iriCollection = iriService.GetLocalActorIris(iriService.InstanceActorIri);

            var expandedDocument = new ActivityPubJsonBuilder()
                .Id(iriService.InstanceActorIri.Iri)
                .Type(JsonLdTypes.PERSON)
                .Inbox(iriCollection.Inbox.Iri)
                .Outbox(iriCollection.Outbox.Iri)
                .Followers(iriCollection.Followers.Iri)
                .Following(iriCollection.Following.Iri)
                .PublicKeyPem(iriCollection.PublicKey.Iri, iriService.InstanceActorIri.Iri, publicKeyPem)
                .Build();


            return await jsonLdService.CompactAsync(NoSignatureAuthor.Instance, expandedDocument);
        }

        public Task<bool> IsInASigningMoodAsync() => Task.FromResult(options.Value.SignRequests);
    }
}
