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
        [Id(0)]
        public string Json;
    }

    [RegisterConverter]
    public sealed class JObjectConverter : IConverter<JObject, JObjectSurrogate>
    {
        public JObject ConvertFromSurrogate(in JObjectSurrogate surrogate)
        {
            return JObject.Parse(surrogate.Json);
        }

        public JObjectSurrogate ConvertToSurrogate(in JObject value)
        {
            return new JObjectSurrogate { Json = value.ToString(Newtonsoft.Json.Formatting.None) };
        }
    }
}
