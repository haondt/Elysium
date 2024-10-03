using Elysium.Core.Converters;
using Elysium.Core.Exceptions;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Elysium.Core.Extensions
{
    public static class JsonExtensions
    {
        private static Exception Error = new JsonNavigationException("object was not in the expected format");
        public static T Get<T>(this JObject jObject, string name) where T : JToken
        {
            var result = jObject[name];
            return TypeCoercer.Coerce<T>(result);
        }
        public static bool Is<T>(this JToken jtoken) where T : JToken
        {
            return jtoken is T;
        }
        public static bool Is<T>(this JToken jtoken, [NotNullWhen(true)] out T? TValue) where T : JToken
        {
            if (jtoken is T casted)
            {
                TValue = casted;
                return true;
            }

            TValue = default;
            return false;
        }

        public static T As<T>(this JToken jtoken) where T : JToken
        {
            return TypeCoercer.Coerce<T>(jtoken);
        }

        public static TResult As<TToken, TResult>(this TToken jtoken) where TToken : JToken where TResult : JToken
        {
            return TypeCoercer.Coerce<TResult>(jtoken);
        }

        public static string AsString(this JToken jtoken)
        {
            var jv = jtoken.As<JToken>();
            if (jv.Type != JTokenType.String) throw new InvalidCastException($"Cannot convert jvalue of type {jv.Type} to string");
            return jv.ToString();
        }

        public static T SetDefault<T>(this JObject target, string key, Func<T> defaultValueFactory) where T : JToken
        {
            if (!target.ContainsKey(key))
                target[key] = defaultValueFactory();
            return TypeCoercer.Coerce<T>(target[key]);
        }

        public static TValue SetDefault<TFiller, TValue>(this JArray target, int index, Func<TFiller> fillerValueFactory, Func<TValue> defaultValueFactory)
            where TFiller : JToken
            where TValue : JToken
        {
            while (target.Count < index)
                target.Add(fillerValueFactory());
            if (target.Count == index)
                target.Add(defaultValueFactory());
            return TypeCoercer.Coerce<TValue>(target[index]);
        }
    }

}
