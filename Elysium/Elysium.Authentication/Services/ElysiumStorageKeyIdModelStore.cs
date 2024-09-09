using Elysium.Core.Models;
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
    public class ElysiumStorageKeyIdModelStore<T>(IStorage storage) where T : class, IStorageKeyIdModel<T>
    {
        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            var hasUser = await storage.ContainsKey(user.Id);
            if (hasUser)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "User already exists" });
            await storage.Set(user.Id, user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            _ = await storage.Delete(user.Id);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<T?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var storageKey = StorageKeyConvert.Deserialize<T>(userId);
            var user = await storage.Get(storageKey);
            if (!user.IsSuccessful)
                return null;
            return user.Value;
        }


        public async Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            var hasUser = await storage.ContainsKey(user.Id);
            if (!hasUser)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "User does not exist" });
            await storage.Set(user.Id, user);
            return IdentityResult.Success;
        }
    }
}
