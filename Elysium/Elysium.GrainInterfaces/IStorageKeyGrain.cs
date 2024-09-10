using Elysium.Core.Models;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IStorageKeyGrain<T> : IGrain<StorageKey<T>>
    {
        public Task<Result<T, StorageResultReason>> GetAsync();
        public Task<bool> ExistsAsync();
        public Task SetAsync(T value);
        Task ClearAsync();
    }

    //public interface IUserIdentityStorageKeyGrain : IStorageKeyGrain<UserIdentity> { }
    //public interface IUserIdentityStorageKeyGrain : IGrain<StorageKey>
    //{
    //    public Task<Result<T>> GetIdentityAsync<T>();

    //}
}
