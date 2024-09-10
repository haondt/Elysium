using Elysium.ActivityPub.Extensions;
using Elysium.ActivityPub.Models;
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
    public class ActivityPubJsonBuilder
    {
        private JArray _state = [];
        private static Func<JObject> JObjectFactory = () => new();
        private static Func<JArray> JArrayFactory = () => new();

        public ActivityPubJsonBuilder Type(string type)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                ["@type"] = new JArray { type };
            return this;
        }
        
        public ActivityPubJsonBuilder Id(Iri id)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                ["@id"] = new JArray { id.ToString() };
            return this;
        }
        private ActivityPubJsonBuilder SetKeyValue(string key, string value)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                [key] = new JArray { new JObject { { "@value", value } } };
            return this;
        }
        private ActivityPubJsonBuilder SetKeyId(string key, string value)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                [key] = new JArray { new JObject { { "@id", value } } };
            return this;
        }

        private ActivityPubJsonBuilder SetKeyIds(string key, IEnumerable<string> values)
        {
            var jArray = new JArray();
            foreach (var value in values)
                jArray.Add(new JObject { { "@id", value } });
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                [key] = jArray;
            return this;
        }

        private ActivityPubJsonBuilder ClearKey(string key)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                .Remove(key);
            return this;
        }

        public ActivityPubJsonBuilder Inbox(Iri iri) => SetKeyId(JsonLdTypes.INBOX, iri.ToString());
        public ActivityPubJsonBuilder Outbox(Iri iri) => SetKeyId(JsonLdTypes.OUTBOX, iri.ToString());
        public ActivityPubJsonBuilder Followers(Iri iri) => SetKeyId(JsonLdTypes.FOLLOWERS, iri.ToString());
        public ActivityPubJsonBuilder Following(Iri iri) => SetKeyId(JsonLdTypes.FOLLOWING, iri.ToString());
        public ActivityPubJsonBuilder PreferredUsername(string username) => SetKeyValue(JsonLdTypes.PREFERRED_USERNAME, username);
        public ActivityPubJsonBuilder AttributedTo(Iri iri) => SetKeyId(JsonLdTypes.ATTRIBUTED_TO, iri.ToString());
        public ActivityPubJsonBuilder Content(string content) => SetKeyValue(JsonLdTypes.CONTENT, content);
        public ActivityPubJsonBuilder Cc(Iri iri) => SetKeyId(JsonLdTypes.CC, iri.ToString());
        public ActivityPubJsonBuilder To(Iri iri) => SetKeyId(JsonLdTypes.TO, iri.ToString());
        public ActivityPubJsonBuilder To(List<Iri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.TO) : SetKeyIds(JsonLdTypes.TO, uris.Select(iri => iri.ToString()));
        public ActivityPubJsonBuilder Cc(List<Iri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.CC) : SetKeyIds(JsonLdTypes.CC, uris.Select(iri => iri.ToString()));
        public ActivityPubJsonBuilder Bto(List<Iri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.BTO) : SetKeyIds(JsonLdTypes.BTO, uris.Select(iri => iri.ToString()));
        public ActivityPubJsonBuilder Bcc(List<Iri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.BCC) : SetKeyIds(JsonLdTypes.BCC, uris.Select(iri => iri.ToString()));
        public ActivityPubJsonBuilder Published(DateTime dateTime)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                [JsonLdTypes.PUBLISHED] = new JArray
                {
                    new JObject
                    {
                        { "@type", JsonLdTypes.DATETIME },
                        { "@value", dateTime.AsXsdString() }
                    }
                };
            return this;
        }
        public ActivityPubJsonBuilder Object(Iri iri) => SetKeyId(JsonLdTypes.OBJECT, iri.ToString());
        public ActivityPubJsonBuilder Actor(Iri iri) => SetKeyId(JsonLdTypes.ACTOR, iri.ToString());
        public JArray Build()
        {
            return _state;
        }
    }

}
