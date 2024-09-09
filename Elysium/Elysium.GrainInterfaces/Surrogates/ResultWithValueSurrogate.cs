using Haondt.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct ResultWithValueSurrogate<TValue, TReason>
    {
        [Id(0)]
        public TReason? Reason;
        [Id(1)]
        public bool IsSuccessful;
        [Id(2)]
        public TValue? Value;
    }

    [RegisterConverter]
    public sealed class ResultConverter<TValue, TReason> : IConverter<Result<TValue, TReason>, ResultWithValueSurrogate<TValue, TReason>>
    {
        public Result<TValue, TReason> ConvertFromSurrogate(in ResultWithValueSurrogate<TValue, TReason> surrogate)
        {
            if (surrogate.IsSuccessful)
                return new(surrogate.Value!);
            return new(surrogate.Reason!);
        }

        public ResultWithValueSurrogate<TValue, TReason> ConvertToSurrogate(in Result<TValue, TReason> value)
        {
            if (value.IsSuccessful)
                return new() { IsSuccessful = true, Reason = default, Value = value.Value };
            return new() { IsSuccessful = false, Reason = value.Reason, Value = default };
        }
    }
}
