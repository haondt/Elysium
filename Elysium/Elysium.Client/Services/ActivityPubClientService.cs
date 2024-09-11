using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Services;
using Haondt.Identity.StorageKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{
    public class ActivityPubClientService(
        IIriService iriService,
        IStorageKeyGrainFactory<UserIdentity> userIdentityGrainFactory,
        IGrainFactory<LocalIri> grainFactory) : IActivityPubClientService
    {
        public async Task<LocalIri> GetLocalIriFromUserIdentityAsync(StorageKey<UserIdentity> identity)
        {
            var userIdentityGrain = userIdentityGrainFactory.GetGrain(identity);
            var userIdentity = await userIdentityGrain.GetAsync();
            if (!userIdentity.IsSuccessful)
                throw new UnauthorizedAccessException($"Unable to retrieve user identity {identity}");

            var localizedUsername = userIdentity.Value.LocalizedUsername ?? identity.Parts[^1].Value;
            return iriService.GetIriForLocalizedActorname(localizedUsername);
        }

        public async Task<(LocalIri ActivityUri, LocalIri ObjectUri)> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, JArray expandedObject)
        {
            var userUri = await GetLocalIriFromUserIdentityAsync(author);
            var userGrain = grainFactory.GetGrain<ILocalActorGrain>(userUri);

            return await userGrain.PublishActivity(type, expandedObject);
        }
    }
}
