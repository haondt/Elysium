﻿using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
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
        IStorageKeyGrainFactory<UserIdentity> userIdentityGrainFactory,
        IGrainFactory<LocalIri> grainFactory) : IActivityPubClientService
    {
        public async Task<(LocalIri ActivityUri, LocalIri ObjectUri)> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, JArray @object)
        {
            var userIdentityGrain = userIdentityGrainFactory.GetGrain(author);
            var userIdentity = await userIdentityGrain.GetAsync();
            if (!userIdentity.IsSuccessful)
                throw new UnauthorizedAccessException($"Unable to retrieve user identity {author}");

            var localizedUsername = userIdentity.Value.LocalizedUsername ?? author.Parts[^1].Value;
            var userUri = hostingService.GetIriForLocalizedUsername(localizedUsername);
            var userGrain = grainFactory.GetGrain<ILocalActorGrain>(userUri);

            return await userGrain.PublishActivity(type, @object);
        }
    }
}
