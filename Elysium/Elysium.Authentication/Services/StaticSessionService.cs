using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class StaticSessionService(Optional<StorageKey<UserIdentity>> identity) : ISessionService
    {
        public void ClearCache() { }

        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync() => Task.FromResult(identity);

        public bool IsAuthenticated() => identity.HasValue;
    }
}
