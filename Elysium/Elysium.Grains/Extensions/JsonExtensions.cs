using DotNext;
using Elysium.Grains.Exceptions;
using Elysium.Grains.Extensions;
using Newtonsoft.Json.Linq;

namespace Elysium.Grains.Extensions
{
    public static class JsonExtensions
    {
        private static Exception Error = new ActivityPubException("object was not in the expected format");
        public static Result<JToken> GetNamedChild(this JToken jtoken, string name)
        {
            if (jtoken is not JObject jo) return new(Error);
            if (!jo.TryGetValue(name, out var value)) return new(Error);
            return value;
        }
        public static Result<JToken> GetNamedChild(this Result<JToken> jtoken, string name)
        {
            if (!jtoken.IsSuccessful) return jtoken;
            return jtoken.Value.GetNamedChild(name);
        }
        public static Result<JToken> Single(this Result<JToken> jtoken)
        {
            if (!jtoken.IsSuccessful) return jtoken;
            if (jtoken.Value is not JArray ja) return new(Error);
            if (ja.Count != 1) return new(Error);
            return new(ja.Single());
        }
        public static Result<string> AsString(this Result<JToken> jtoken)
        {
            if (!jtoken.IsSuccessful) return new(jtoken.Error);
            if (jtoken.Value is not JValue jv) return new(Error);
            if (jv.Type != JTokenType.String) return new(Error);
            return jv.ToString();
        }
        public static Result<bool> ShouldBe<T>(this Result<T> target, T value) where T : JToken, IEquatable<T>
        {
            if (!target.IsSuccessful) return new(target.Error);
            return new(target.Value.Equals(value));
        }
        public static Optional<Exception> Set<T>(this Result<JObject> target, string key, T Value) where T : JToken
        {
            if (!target.IsSuccessful) return new(target.Error);
            target.Value[key] = Value;
            return new();
        }
        public static Result<T> SetDefault<T>(this Result<JObject> target, string key, Func<T> defaultValueFactory) where T : JToken
        {
            if (!target.IsSuccessful) return new(target.Error);
            if (!target.Value.ContainsKey(key))
                target.Value[key] = defaultValueFactory();
            if (target.Value[key] is not T castedValue) return new(Error);
            return castedValue;
        }
        public static Result<TValue> SetDefault<TFiller, TValue>(this Result<JArray> target, int index, Func<TFiller> fillerValueFactory, Func<TValue> defaultValueFactory)
            where TFiller : JToken
            where TValue : JToken
        {
            if (!target.IsSuccessful) return new(target.Error);
            while (target.Value.Count < index)
                target.Value.Add(fillerValueFactory());
            if (target.Value.Count == index)
                target.Value[index] = defaultValueFactory();
            if (target.Value[index] is not TValue castedValue) return new(Error);
            return castedValue;
        }
    }

}
