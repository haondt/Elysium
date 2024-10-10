using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Haondt.Identity.StorageKey;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;

namespace Elysium.Client.Services
{
    public class ActivityPubClientService(
        IIriService iriService,
        UserManager<UserIdentity> userManager,
        IStorageKeyGrainFactory<UserIdentity> userIdentityGrainFactory,
        IGrainFactory<LocalIri> grainFactory) : IActivityPubClientService
    {
        public async Task<LocalIri> GetLocalIriFromUserIdentityAsync(StorageKey<UserIdentity> identity)
        {
            var userIdentityGrain = userIdentityGrainFactory.GetGrain(identity);
            var userIdentity = await userIdentityGrain.GetAsync();
            if (!userIdentity.IsSuccessful)
                throw new UnauthorizedAccessException($"Unable to retrieve user identity {identity}");

            var localizedUsername = userIdentity.Value.LocalizedUsername
                // ?? identity.Parts[^1].Value; this is foo@bar.com, todo: convert it all the way to foo
                ?? throw new UnauthorizedAccessException("Unable to retrieve localized username");
            return iriService.GetIriForLocalizedActorname(localizedUsername);
        }

        public async Task<StorageKey<UserIdentity>> GetUserIdentityFromLocalIriAsync(LocalIri iri)
        {
            var localizedActorname = iriService.GetLocalizedActornameForLocalIri(iri);
            var identityUser = await userManager.FindByNameAsync(localizedActorname)
                ?? throw new KeyNotFoundException($"Unable to retrieve user with name {localizedActorname}");
            return identityUser.Id;
        }

        public async Task<(LocalIri ActivityUri, JObject Activity)> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, JArray expandedObject)
        {
            var userUri = await GetLocalIriFromUserIdentityAsync(author);
            var userGrain = grainFactory.GetGrain<ILocalActorGrain>(userUri);

            return await userGrain.PublishActivity(type, expandedObject);
        }
    }
}
