using Elysium.ActivityPub.Helpers;
using Elysium.Authentication.Services;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class ActorDocumentGenerator(IJsonLdService jsonLdService,
        IIriService iriService,
        IGrainFactory instanceActorGrainFactory,
        IUserCryptoService cryptoService,
        IGrainFactory<LocalIri> actorGrainFactory) : IActorDocumentGenerator
    {
        public async Task<JObject> GenerateAsync(LocalIri target)
        {
            var grain = actorGrainFactory.GetGrain<ILocalActorGrain>(target);
            var grainState = await grain.GetStateAsync();
            var publicKeyPem = cryptoService.EncodePublicKeyToPemX509(grainState.PublicKey);
            var iriCollection = iriService.GetLocalActorIris(target);

            var expandedDocument = new ActivityPubJsonBuilder()
                .Id(target.Iri)
                .Type(grainState.Type)
                .Inbox(iriCollection.Inbox.Iri)
                .Outbox(iriCollection.Outbox.Iri)
                .Followers(iriCollection.Followers.Iri)
                .Following(iriCollection.Following.Iri)
                .PublicKeyPem(iriCollection.PublicKey.Iri, target.Iri, publicKeyPem)
                .Build();

            var instanceActor = instanceActorGrainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);

            return await jsonLdService.CompactAsync(instanceActor, expandedDocument);
        }
    }
}
