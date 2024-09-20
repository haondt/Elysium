using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Persistence.Exceptions;
using Elysium.Persistence.Services;
using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain
{

    public class StorageKeyGrain<T> : Grain, IStorageKeyGrain<T>
    {
        private readonly StorageKey<T> _id;
        private Result<T, StorageResultReason> _value;
        private readonly IStorageKeyGrainFactory<T> _grainFactory;
        private readonly IElysiumStorage _storage;

        public StorageKeyGrain(
            IStorageKeyGrainFactory<T> grainFactory,
            IElysiumStorage storage)
        {
            _id = grainFactory.GetIdentity(this);
            _grainFactory = grainFactory;
            _storage = storage;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _value = await _storage.Get(_id);

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
            return _storage.Set(_id, value);
        }

        public async Task ClearAsync()
        {
            if (!_value.IsSuccessful && _value.Reason == StorageResultReason.NotFound)
                return;
            var result = await _storage.Delete(_id);
            if (!result.IsSuccessful)
                if (result.Reason != StorageResultReason.NotFound)
                {
                    _value = await _storage.Get(_id);
                    throw new StorageException($"Failed to delete stored document with id {_id}");
                }
            _value = await _storage.Get(_id);
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
