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
        public string Json { get; set; }
    }

    [RegisterConverter]
    public sealed class JArrayConverter : IConverter<JArray, JArraySurrogate>
    {
        public JArray ConvertFromSurrogate(in JArraySurrogate surrogate)
        {
            return JArray.FromObject(surrogate);
        }

        public JArraySurrogate ConvertToSurrogate(in JArray value)
        {
            return new JArraySurrogate { Json = value.ToString(Newtonsoft.Json.Formatting.None) };
        }
    }
}
