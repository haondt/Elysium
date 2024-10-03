using Newtonsoft.Json.Linq;

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
