using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Converters
{
    public static class TypeCoercer
    {
        public static T Coerce<T>(object? value)
        {
            if (value is T casted)
                return casted;
            throw new InvalidCastException($"Cannot coerce object of type {value?.GetType()} to type {typeof(T)}");
        }
    }
}
