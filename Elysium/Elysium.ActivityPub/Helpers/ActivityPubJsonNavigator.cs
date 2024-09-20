using Elysium.ActivityPub.Models;
using Elysium.Core.Exceptions;
using Elysium.Core.Extensions;
using Elysium.Core.Models;
using Haondt.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.ActivityPub.Helpers
{
    public static class ActivityPubJsonNavigator
    {
        public static Iri GetInbox(JArray expanded)
        {
            var uriString = expanded
                .Single()
                .As<JObject>()
                .Get<JObject>(JsonLdTypes.INBOX)
                .Get<JValue>("@id")
                .AsString();

            return Iri.FromUnencodedString(uriString);
        }

        public static JToken GetValue(JArray expanded, string key)
        {
            return expanded
                .Single()
                .As<JObject>()
                .Get<JArray>(key)
                .Single()
                .As<JObject>()
                ["@value"]!;
        }
        public static Optional<string> TryGetValue(JArray expanded, string key)
        {
            if (expanded.Count == 1
                && expanded.Single() is JObject jo
                && jo.TryGetValue(key, out var value)
                && value is JArray ja
                && ja.Count == 1
                && ja.Single() is JObject jo2
                && jo2.TryGetValue("@value", out var value2)
                && value2 is JValue jv
                && jv.Type == JTokenType.String)
                return new(jv.ToString());
            return new();
        }

        public static bool IsActor(JArray actor)
        {
            var mainObject =  actor[0].As<JObject>();
            return mainObject.ContainsKey(JsonLdTypes.INBOX)
                && mainObject.ContainsKey(JsonLdTypes.OUTBOX)
                && mainObject.ContainsKey(JsonLdTypes.FOLLOWERS)
                && mainObject.ContainsKey(JsonLdTypes.FOLLOWING);
        }


        /// <summary>
        /// The object's unique global identifier (unless the object is transient, in which case the id MAY be omitted).
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public static string GetId(JArray expandedTarget)
        {
            return expandedTarget
                .Single()
                .As<JObject>()
                .Get<JValue>("@id")
                .AsString();
        }
        public static string GetId(JArray expandedTarget, string key)
        {
            return expandedTarget
                .Single()
                .As<JObject>()
                .Get<JArray>(key)
                .Single()
                .As<JObject>()
                .Get<JValue>("@id")
                .AsString();
        }

        /// <summary>
        /// The type of the object. All objects MUST have this.
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public static string GetType(JArray expanded)
        {
            return expanded
                .Single()
                .As<JObject>()
                .Get<JArray>("@type")
                .Single()
                .AsString();
        }

        public static Optional<string> TryGetPreferredUsername(JArray expanded)
        {
            return TryGetValue(expanded, JsonLdTypes.PREFERRED_USERNAME);
        }

        /// <summary>
        /// return the object field
        /// </summary>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public static JArray GetObject(JArray expanded)
        {
            return expanded
                .Single()
                .As<JObject>()
                .Get<JArray>(JsonLdTypes.OBJECT)
                .As<JArray>();
        }

        public static Optional<DateTime> TryGetPublished(JArray expanded)
        {
            if (expanded.Count != 1
                || expanded.Single() is not JObject jo
                || !jo.TryGetValue(JsonLdTypes.PUBLISHED, out var value)
                || value is not JArray ja
                || ja.Count != 1
                || ja.Single() is not JObject publishedObj)
                return new();
            if (!publishedObj.TryGetValue("@type", out var publishedTypeToken)
                || publishedTypeToken is not JValue publishedTypeValue
                || publishedTypeValue.Type != JTokenType.String
                || publishedTypeValue.ToString() != JsonLdTypes.DATETIME)
                return new();

            if (!publishedObj.TryGetValue("@value", out var publishedValueToken)
                || publishedValueToken is not JValue publishedValueValue)
                return new();

            if (publishedTypeToken.Type is JTokenType.Date)
                return new((DateTime)publishedTypeToken);
            return new(DateTime.Parse(publishedValueValue.ToString()));
        }


        public static (string PublicKey, PublicKeyType PublicKeyType) GetPublicKey(JArray expanded)
        {
            throw new NotImplementedException();
            //if (expanded.Count != 1) return new(Error);
            //JToken next = expanded.First();
            //var deprecatedStrategy = next.Get(JsonLdTypes.PUBLIC_KEY)
            //    .Get(JsonLdTypes.PUBLIC_KEY_PEM)
            //    .Get("@value")
            //    .AsString();

            //if (deprecatedStrategy.IsSuccessful)
            //    return new((deprecatedStrategy.Value, PublicKeyType.Pem));

            //var updatedStrategy = next.Get(JsonLdTypes.ASSERTION_METHOD)
            //    .Single()
            //    .Get(JsonLdTypes.PUBLIC_KEY_MULTIBASE)
            //    .Get("@value")
            //    .AsString();

            //if (updatedStrategy.IsSuccessful)
            //    return new((updatedStrategy.Value, PublicKeyType.Multibase));
            //return new(updatedStrategy.Error);
        }

    }

}
