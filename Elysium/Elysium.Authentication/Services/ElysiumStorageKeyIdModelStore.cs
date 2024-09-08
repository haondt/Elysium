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
            var getUser = await storage.ContainsKey(user.Id);
            if (!getUser.IsSuccessful)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "Could not check if user exists" });
            if (getUser.Value)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "User already exists" });
            var createUser = await storage.Set(user.Id, user);
            if (createUser.HasValue)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = createUser.Value.ToString() });
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            var result = await storage.Delete(user.Id);
            if(result.HasValue)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = result.Value.ToString() });
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
            var getUser = await storage.ContainsKey(user.Id);
            if (!getUser.IsSuccessful)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "Could not check if user exists" });
            if (!getUser.Value)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = "User does not exist" });
            var setUser = await storage.Set(user.Id, user);
            if (setUser.HasValue)
                return IdentityResult.Failed(new IdentityError { Code = "0", Description = setUser.Value.ToString() });
            return IdentityResult.Success;
        }
    }
}
