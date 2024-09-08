using DotNext;
using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Elysium.Server.Services;
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
        IHostingService hostingService,
        IGrainFactory<StorageKey<UserIdentity>> userIdentityGrainFactory,
        IGrainFactory<LocalUri> grainFactory) : IActivityPubClientService
    {
        public async Task<Result<(LocalUri ActivityUri, LocalUri ObjectUri)>> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, JArray @object)
        {
            var userIdentityGrain = userIdentityGrainFactory.GetGrain<IStorageKeyGrain<UserIdentity>>(author);
            var userIdentity = await userIdentityGrain.GetIdentityAsync();
            if (!userIdentity.IsSuccessful)
                return new(userIdentity.Error);

            var localizedUsername = userIdentity.Value.LocalizedUsername;
            if (string.IsNullOrEmpty(localizedUsername))
                return new(new InvalidOperationException("User does not have a localized username"));

            var userUri = hostingService.GetUriForLocalizedUsername(localizedUsername);
            if (!userUri.IsSuccessful)
                return new(userUri.Error);

            var userGrain = grainFactory.GetGrain<ILocalActorGrain>(userUri.Value);

            return await userGrain.PublishActivity(type, @object);
        }
    }
}
