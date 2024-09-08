using DotNext;
using Elysium.Core.Exceptions;
using Elysium.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Elysium.Core.Extensions
{
    public static class JsonExtensions
    {
        private static Exception Error = new JsonNavigationException("object was not in the expected format");
        public static Result<JToken> Get(this JToken jtoken, string name)
        {
            if (jtoken is not JObject jo) return new(Error);
            if (!jo.TryGetValue(name, out var value)) return new(Error);
            return value;
        }
        public static Result<bool> Is<T>(this Result<JToken> jtoken) where T : JToken
        {
            if (!jtoken.IsSuccessful) return new Result<bool>(jtoken.Error);
            return jtoken.Value is T;
        }
        public static Result<TResult> As<TResult>(this Result<JToken> jtoken) where TResult : JToken
        {
            if (!jtoken.IsSuccessful) return new Result<TResult>(jtoken.Error);
            if (jtoken.Value is not TResult tValue) return new Result<TResult>(new InvalidCastException($"Cannot cast object of type {jtoken.Value.GetType()} to {nameof(TResult)}"));
            return tValue;
        }

        public static Result<TResult> As<TToken, TResult>(this Result<TToken> jtoken) where TToken : JToken where TResult : JToken
        {
            if (!jtoken.IsSuccessful) return new Result<TResult>(jtoken.Error);
            if (jtoken.Value is not TResult tValue) return new Result<TResult>(new InvalidCastException($"Cannot cast object of type {jtoken.Value.GetType()} to {nameof(TResult)}"));
            return tValue;
        }

        public static Result<IEnumerable<TResult>> Select<T, TResult>(this Result<T> jtoken, Func<JToken, TResult> predicate) where T : JToken
        {
            var casted = jtoken.As<T, JArray>();
            if (!casted.IsSuccessful) return new(casted.Error);
            try { return new(casted.Value.Select(q => predicate(q))); }
            catch (Exception ex) { return new Result<IEnumerable<TResult>>(ex); }
        }

        public static Result<IEnumerable<JToken>> Where<T>(this Result<T> jtoken, Func<JToken, bool> predicate) where T : JToken
        {
            var casted = jtoken.As<T, JArray>();
            if (!casted.IsSuccessful) return new(casted.Error);
            try { return new(casted.Value.Where(q => predicate(q))); }
            catch (Exception ex) { return new Result<IEnumerable<JToken>>(ex); }
        }

        public static Result<JToken> First<T>(this Result<T> jtoken, Func<JToken, bool>? predicate = null) where T : JToken
        {
            var casted = jtoken.As<T, JArray>();
            if (!casted.IsSuccessful) return new(casted.Error);
            if (casted.Value.Count == 0)
                return new(new InvalidOperationException("JArray has zero items"));
            try
            {
                if (predicate != null)
                    return new(casted.Value.First(predicate));
                return new(casted.Value.First());
            }
            catch (Exception ex) { return new Result<JToken>(ex); }
        }

        public static Result<JToken> Get(this Result<JToken> jtoken, string name)
        {
            if (!jtoken.IsSuccessful) return jtoken;
            return jtoken.Value.Get(name);
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
        public static Result<bool> Equals<T>(this Result<T> target, T value) where T : JToken, IEquatable<T>
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

        public static Result<bool> Remove(this Result<JObject> target, string key)
        {
            if (!target.IsSuccessful) return new(target.Error);
            if (!target.Value.ContainsKey(key)) return new(false);
            return new(target.Value.Remove(key));
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
                target.Value.Add(defaultValueFactory());
            if (target.Value[index] is not TValue castedValue) return new(Error);
            return castedValue;
        }
    }

}
