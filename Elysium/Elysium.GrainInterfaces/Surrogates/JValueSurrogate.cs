using Newtonsoft.Json.Linq;

namespace Elysium.GrainInterfaces.Surrogates
{
    [GenerateSerializer]
    public struct JValueSurrogate
    {
        [Id(0)]
        public string JsonValue;
    }

    [RegisterConverter]
    public sealed class JValueConverter : IConverter<JValue, JValueSurrogate>
    {
        public JValue ConvertFromSurrogate(in JValueSurrogate surrogate)
        {
            return JValue.Parse(surrogate.JsonValue) as JValue ?? throw new ArgumentException(surrogate.JsonValue);
        }

        public JValueSurrogate ConvertToSurrogate(in JValue value)
        {
            return new JValueSurrogate { JsonValue = value.ToString(Newtonsoft.Json.Formatting.None) };
        }
    }
}
