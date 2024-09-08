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
    public interface IUserIdentityGrain : IGrain<StorageKey<UserIdentity>>
    {
        public Task<Result<UserIdentity>> GetIdentityAsync();
    }
}
