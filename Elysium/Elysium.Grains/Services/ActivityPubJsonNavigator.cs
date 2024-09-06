using DotNext;
using Elysium.Grains.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public class ActivityPubJsonNavigator : IActivityPubJsonNavigator
    {

        private static Exception Error = new ActivityPubException("object was not in the expected format");

        public Result<Uri> GetInbox(JArray expanded)
        {
            if (expanded.Count != 1) return new(Error);
            JToken next = expanded.First();
            var result = next.GetNamedChild("http://www.w3.org/ns/ldp#inbox")
                .GetNamedChild("@id")
                .AsString();

            if (!result.IsSuccessful)
                return new(result.Error);
            return new(new Uri(result.Value));
        }


        /// <summary>
        /// The object's unique global identifier (unless the object is transient, in which case the id MAY be omitted).
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public Optional<string> GetId(JObject target)
        {
            return target.GetNamedChild("id").AsString();
        }

        /// <summary>
        /// The type of the object. All objects MUST have this.
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public Result<string> GetType(JObject target)
        {
            return target.GetNamedChild("type").AsString();
        }

        public Result<(string, PublicKeyType)> GetPublicKey(JArray expanded)
        {
            if (expanded.Count != 1) return new(Error);
            JToken next = expanded.First();
            var deprecatedStrategy = next.GetNamedChild("https://w3id.org/security#publicKey")
                .GetNamedChild("https://w3id.org/security#publicKeyPem")
                .GetNamedChild("@value")
                .AsString();

            if (deprecatedStrategy.IsSuccessful)
                return new((deprecatedStrategy.Value, PublicKeyType.Pem));

            var updatedStrategy = next.GetNamedChild("https://w3id.org/security#assertionMethod")
                .Single()
                .GetNamedChild("https://w3id.org/security#publicKeyMultibase")
                .GetNamedChild("@value")
                .AsString();

            if (updatedStrategy.IsSuccessful)
                return new((updatedStrategy.Value, PublicKeyType.Multibase));
            return new(updatedStrategy.Error);
        }

    }

    public static class JsonExtensions
    {
        private static Exception Error = new ActivityPubException("object was not in the expected format");
        public static Result<JToken> GetNamedChild(this JToken jtoken, string name)
        {
            if (jtoken is not JObject jo) return new (Error);
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
    }

}
