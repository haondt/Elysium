using DotNext;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public interface IStorageKeyGrain<T> : IGrain<StorageKey<T>>
    {
        public Task<Result<T>> GetAsync();
    }

    //public interface IUserIdentityStorageKeyGrain : IStorageKeyGrain<UserIdentity> { }
    //public interface IUserIdentityStorageKeyGrain : IGrain<StorageKey>
    //{
    //    public Task<Result<T>> GetIdentityAsync<T>();

    //}
}
