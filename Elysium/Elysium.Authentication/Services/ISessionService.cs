using DotNext;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public interface ISessionService
    {
        public bool IsAuthenticated();
        public Task<Result<StorageKey<UserIdentity>>> GetUserKeyAsync();
        public void ClearCache();
    }
}
