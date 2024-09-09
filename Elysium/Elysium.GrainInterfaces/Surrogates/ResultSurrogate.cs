using Haondt.Core.Models;
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
        public T? Reason;
        [Id(1)]
        public bool IsSuccessful;
    }

    [RegisterConverter]
    public sealed class ResultConverter<T> : IConverter<Result<T>, ResultSurrogate<T>>
    {
        public Result<T> ConvertFromSurrogate(in ResultSurrogate<T> surrogate)
        {
            if (surrogate.IsSuccessful)
                return new();
            return new(surrogate.Reason!);
        }

        public ResultSurrogate<T> ConvertToSurrogate(in Result<T> value)
        {
            if (value.IsSuccessful)
                return new() { IsSuccessful = true, Reason = default };
            return new() { IsSuccessful = false, Reason = value.Reason };
        }
    }
}
