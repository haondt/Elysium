using Haondt.Core.Models;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct OptionalSurrogate<T>
    {
        [Id(0)]
        public T? Value;
        [Id(1)]
        public bool HasValue;
    }

    [RegisterConverter]
    public sealed class OptionalConverter<T> : IConverter<Optional<T>, OptionalSurrogate<T>>
    {
        public Optional<T> ConvertFromSurrogate(in OptionalSurrogate<T> surrogate)
        {
            if (surrogate.HasValue)
                return new(surrogate.Value!);
            return new();
        }

        public OptionalSurrogate<T> ConvertToSurrogate(in Optional<T> value)
        {
            if (value.HasValue)
                return new() { HasValue = true, Value = value.Value };
            return new() { HasValue = false };
        }
    }
}
