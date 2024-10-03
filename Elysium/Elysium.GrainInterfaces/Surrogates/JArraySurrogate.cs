using Newtonsoft.Json.Linq;

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
