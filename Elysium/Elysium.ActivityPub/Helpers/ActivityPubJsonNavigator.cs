using DotNext;
using Elysium.ActivityPub.Models;
using Elysium.Core.Exceptions;
using Elysium.Core.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.ActivityPub.Helpers
{
    public class ActivityPubJsonNavigator
    {

        private static Exception Error = new JsonNavigationException("object was not in the expected format");

        public Result<Uri> GetInbox(JArray expanded)
        {
            if (expanded.Count != 1) return new(Error);
            JToken next = expanded.First();
            //var type = next.GetNamedChild("@type")
            //    .AsString()
            //    .ShouldBe("" // there are no constraints on actor type
            var result = next.Get("http://www.w3.org/ns/ldp#inbox")
                .Get("@id")
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
            return target.Get("id").AsString();
        }

        /// <summary>
        /// The type of the object. All objects MUST have this.
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public Result<string> GetType(JObject target)
        {
            return target.Get("type").AsString();
        }


        public Result<(string PublicKey, PublicKeyType PublicKeyType)> GetPublicKey(JArray expanded)
        {
            if (expanded.Count != 1) return new(Error);
            JToken next = expanded.First();
            var deprecatedStrategy = next.Get(JsonLdTypes.PUBLIC_KEY)
                .Get(JsonLdTypes.PUBLIC_KEY_PEM)
                .Get("@value")
                .AsString();

            if (deprecatedStrategy.IsSuccessful)
                return new((deprecatedStrategy.Value, PublicKeyType.Pem));

            var updatedStrategy = next.Get(JsonLdTypes.ASSERTION_METHOD)
                .Single()
                .Get(JsonLdTypes.PUBLIC_KEY_MULTIBASE)
                .Get("@value")
                .AsString();

            if (updatedStrategy.IsSuccessful)
                return new((updatedStrategy.Value, PublicKeyType.Multibase));
            return new(updatedStrategy.Error);
        }

    }

}
