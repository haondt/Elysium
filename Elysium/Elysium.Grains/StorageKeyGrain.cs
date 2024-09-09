using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Persistence.Services;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{

    public class StorageKeyGrain<T>(
        IGrainFactory<StorageKey<T>> grainFactory,
        IElysiumStorage storage) : Grain, IStorageKeyGrain<T>
    {
        private StorageKey<T> _id;
        private Result<T, StorageResultReason> _value;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _id = grainFactory.GetIdentity(this);
            _value = await storage.Get(_id);

            await base.OnActivateAsync(cancellationToken);
        }

        public Task<Result<T, StorageResultReason>> GetAsync()
        {
            return Task.FromResult(_value);
        }

        public Task<bool> ExistsAsync()
        {
            return Task.FromResult(_value.IsSuccessful);
        }

        public Task SetAsync(T value)
        {
            _value = new(value);
            return storage.Set(_id, value);
        }
    }

    //public class UserIdentityStorageKeyGrain(
    //    IGrainFactory<StorageKey<UserIdentity>> grainFactory,
    //    IElysiumStorage elysiumStorage
    //    ) : StorageKeyGrain<UserIdentity>(grainFactory, elysiumStorage),
    //    IUserIdentityStorageKeyGrain { }
    //public class UserIdentityStorageKeyGrain(
    //    IGrainFactory<StorageKey<UserIdentity>> grainFactory,
    //    IElysiumStorage elysiumStorage
    //    ) : Grain, IUserIdentityStorageKeyGrain
    //{
    //    //private Result<UserIdentity> _identity;

    //    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    //    {
    //        await base.OnActivateAsync(cancellationToken);
    //    }

    //    public Task<Result<T>> GetIdentityAsync<T>()
    //    {
    //        throw new NotImplementedException();
    //    }

    //}
}
