using DotNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct ResultSurrogate<T>
    {
        [Id(0)]
        public T? Value;
        [Id(1)]
        public Exception? Exception;
    }

    [RegisterConverter]
    public sealed class ResultConverter<T> : IConverter<Result<T>, ResultSurrogate<T>>
    {
        public Result<T> ConvertFromSurrogate(in ResultSurrogate<T> surrogate)
        {
            if (surrogate.Exception != null)
                return new(surrogate.Exception);
            if (surrogate.Value != null)
                return new(surrogate.Value);
            throw new OrleansException("Unable to convert surrogate back to result<T>");
        }

        public ResultSurrogate<T> ConvertToSurrogate(in Result<T> value)
        {
            if (value.IsSuccessful)
                return new() { Value = value.Value };
            return new() { Exception = value.Error };
        }
    }
}
