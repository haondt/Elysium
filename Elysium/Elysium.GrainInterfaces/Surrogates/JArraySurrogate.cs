using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct JArraySurrogate
    {
        [Id(0)]
        public string Json;
    }

    [RegisterConverter]
    public sealed class JArrayConverter : IConverter<JArray, JArraySurrogate>
    {
        public JArray ConvertFromSurrogate(in JArraySurrogate surrogate)
        {
            return JArray.Parse(surrogate.Json);
        }

        public JArraySurrogate ConvertToSurrogate(in JArray value)
        {
            return new JArraySurrogate { Json = value.ToString(Newtonsoft.Json.Formatting.None) };
        }
    }
}
