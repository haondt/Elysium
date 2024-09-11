using Elysium.GrainInterfaces;
using Haondt.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains
{
    public class CacheGrain<T> : Grain
    {
        private Optional<T> _value;
        public Task ClearAsync()
        {
            _value = new();
            return Task.CompletedTask;
        }

        public Task<Optional<T>> TryGetValueAsync()
        {
            return Task.FromResult(_value);
        }

        public Task SetValueAsync(T value)
        {
            _value = new(value);
            return Task.CompletedTask;
        }
    }
}
