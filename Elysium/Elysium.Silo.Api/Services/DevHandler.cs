using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Domain.Services;
using Elysium.Hosting.Services;
using Newtonsoft.Json.Linq;
using Haondt.Core.Models;

namespace Elysium.Silo.Api.Services
{
    public class DevHandler(
        IGrainFactory<LocalIri> localGrainFactory,
        IGrainFactory grainFactory,
        IIriService iriService,
        IJsonLdService jsonLdService,
        IUserCryptoService cryptoService) : IDevHandler
    {
        private static Func<JObject> JObjectFactory = () => new();
        public async Task<(LocalIri ActivityIri, JObject Activity)> CreateForLocal(DevLocalActivityPayload payload)
        {
            if ((payload.SubjectObject == null) == string.IsNullOrEmpty(payload.SubjectLink))
                throw new ArgumentException("only one of SubjectObject or SubjectLink must be supplied");

            if (!string.IsNullOrEmpty(payload.SubjectLink))
                throw new ArgumentException("subject link cannot be used for create operation");

            //var actorIri = Iri.FromUnencodedString(payload.ActorIri);

            //var actorIri = new IriBuilder { Host = hostingService.Host, Path = }
            var actorIri = iriService.GetIriForLocalizedActorname(payload.ActorName);

            var actorGrain = localGrainFactory.GetGrain<ILocalActorGrain>(actorIri);
            if (!(await actorGrain.IsInitializedAsync()))
            {
                var (publicKey, privateKey) = cryptoService.GenerateKeyPair();

                await actorGrain.InitializeAsync(new ActorRegistrationDetails
                {
                    PrivateKey = privateKey,
                    PublicKey = publicKey,
                    Type = payload.NewActorType ?? JsonLdTypes.PERSON
                });
            }

            var instanceActor = grainFactory.GetGrain<IInstanceActorAuthorGrain>(Guid.Empty);   
            var expanded = await jsonLdService.ExpandAsync(instanceActor, payload.SubjectObject!);
            expanded.SetDefault(0, JObjectFactory, JObjectFactory)
                [JsonLdTypes.ATTRIBUTED_TO] = new JArray { new JObject { { "@id", actorIri.ToString() } } };
            return await actorGrain.PublishActivity(ActivityType.Create, expanded);
        }
    }
}
