using DotNext;
using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Elysium.GrainInterfaces.Services;
using Elysium.Persistence.Services;
using Haondt.Identity.StorageKey;
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
        private Result<T> _identity;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var storageKey = grainFactory.GetIdentity(this);
            _identity = await storage.Get(storageKey);

            await base.OnActivateAsync(cancellationToken);
        }

        public Task<Result<T>> GetIdentityAsync()
        {
            return Task.FromResult(_identity);
        }
    }
}
