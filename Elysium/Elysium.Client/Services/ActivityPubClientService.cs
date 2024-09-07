using DotNext;
using Elysium.Core.Models;
using Elysium.Server.Models;
using Haondt.Identity.StorageKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{
    public class ActivityPubClientService : IActivityPubClientService
    {
        public Task<Result<Uri>> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, JObject @object)
        {
            throw new NotImplementedException();
        }
    }
}
