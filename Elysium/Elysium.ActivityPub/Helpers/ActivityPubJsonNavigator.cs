using Elysium.ActivityPub.Models;
using Elysium.Core.Exceptions;
using Elysium.Core.Extensions;
using Elysium.Core.Models;
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
        public Iri GetInbox(JArray expanded)
        {
            var uriString = expanded
                .Single()
                .As<JObject>()
                .Get<JObject>(JsonLdTypes.INBOX)
                .Get<JValue>("@id")
                .AsString();

            return Iri.FromUnencodedString(uriString);
        }


        /// <summary>
        /// The object's unique global identifier (unless the object is transient, in which case the id MAY be omitted).
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public string GetId(JObject target)
        {
            //return target.Get<JValue>("id").AsString();
            throw new NotImplementedException();
        }

        /// <summary>
        /// The type of the object. All objects MUST have this.
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#obj-id"/></remarks>
        /// <param name="expanded"></param>
        /// <returns></returns>
        public string GetType(JObject target)
        {
            throw new NotImplementedException();
            //return target.Get("type").AsString();
        }


        public (string PublicKey, PublicKeyType PublicKeyType) GetPublicKey(JArray expanded)
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
