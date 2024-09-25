using Elysium.Core.Models;
using Elysium.Core.Services;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class ElysiumStorageKeyIdModelStore<T>(IElysiumStorage storage, IElysiumStorageKeyConverter converter) where T : class, IStorageKeyIdModel<T>
    {
        protected readonly IElysiumStorage _storage = storage;

        public virtual async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            var hasUser = await _storage.ContainsKey(user.Id);
            if (hasUser)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "User already exists" });
            await _storage.Set(user.Id, user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            _ = await _storage.Delete(user.Id);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<T?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var storageKey = converter.Deserialize<T>(userId);
            var user = await _storage.Get(storageKey);
            if (!user.IsSuccessful)
                return null;
            return user.Value;
        }


        public virtual async Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            var hasUser = await _storage.ContainsKey(user.Id);
            if (!hasUser)
                return IdentityResult.Failed(new IdentityError { Code = "1", Description = "User does not exist" });
            await _storage.Set(user.Id, user);
            return IdentityResult.Success;
        }
    }
}
