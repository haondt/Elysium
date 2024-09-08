using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct JObjectSurrogate
    {
        public string Json { get; set; }
    }

    [RegisterConverter]
    public sealed class JObjectConverter : IConverter<JObject, JObjectSurrogate>
    {
        public JObject ConvertFromSurrogate(in JObjectSurrogate surrogate)
        {
            return JObject.FromObject(surrogate);
        }

        public JObjectSurrogate ConvertToSurrogate(in JObject value)
        {
            return new JObjectSurrogate { Json = value.ToString(Newtonsoft.Json.Formatting.None) };
        }
    }
}
