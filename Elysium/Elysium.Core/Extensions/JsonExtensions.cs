using Elysium.Core.Exceptions;
using Elysium.Core.Extensions;
using Newtonsoft.Json;
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
            if (result is not T castedValue) throw new InvalidCastException($"Cannot cast object of type {result?.GetType()} to {typeof(T)}");
            return castedValue;
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
            if (jtoken is not T tValue) throw new InvalidCastException($"Cannot cast object of type {jtoken.GetType()} to {typeof(T)}");
            return tValue;
        }

        public static TResult As<TToken, TResult>(this TToken jtoken) where TToken : JToken where TResult : JToken
        {
            if (jtoken is not TResult tValue) throw new InvalidCastException($"Cannot cast object of type {jtoken.GetType()} to {nameof(TResult)}");
            return tValue;
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
            if (target[key] is not T castedValue) throw new InvalidCastException($"existing value has type {target[key]?.GetType()} but was expecting {typeof(T)}");
            return castedValue;
        }

        public static TValue SetDefault<TFiller, TValue>(this JArray target, int index, Func<TFiller> fillerValueFactory, Func<TValue> defaultValueFactory)
            where TFiller : JToken
            where TValue : JToken
        {
            while (target.Count < index)
                target.Add(fillerValueFactory());
            if (target.Count == index)
                target.Add(defaultValueFactory());
            if (target[index] is not TValue castedValue) throw new InvalidCastException($"existing value has type {target[index]?.GetType()} but was expecting {typeof(TValue)}");
            return castedValue;
        }
    }

}
