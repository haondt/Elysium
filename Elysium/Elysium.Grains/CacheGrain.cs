using Haondt.Core.Models;

namespace Elysium.Domain
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
