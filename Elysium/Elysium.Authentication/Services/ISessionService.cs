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
    public interface ISessionService
    {
        public bool IsAuthenticated();
        public Task<Optional<StorageKey<UserIdentity>>> GetUserKeyAsync();
        public void ClearCache();
        Optional<T> GetFromCookie<T>(string key);
        void SetCookie<T>(string key, T value);
    }
}
