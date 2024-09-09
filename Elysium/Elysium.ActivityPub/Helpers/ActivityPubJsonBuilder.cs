using Elysium.ActivityPub.Extensions;
using Elysium.ActivityPub.Models;
using Elysium.Core.Extensions;
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
        
        public ActivityPubJsonBuilder Id(Uri id)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                ["@id"] = new JArray { id.AbsoluteUri };
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

        public ActivityPubJsonBuilder Inbox(Uri uri) => SetKeyId(JsonLdTypes.INBOX, uri.AbsoluteUri);
        public ActivityPubJsonBuilder Outbox(Uri uri) => SetKeyId(JsonLdTypes.OUTBOX, uri.AbsoluteUri);
        public ActivityPubJsonBuilder Followers(Uri uri) => SetKeyId(JsonLdTypes.FOLLOWERS, uri.AbsoluteUri);
        public ActivityPubJsonBuilder Following(Uri uri) => SetKeyId(JsonLdTypes.FOLLOWING, uri.AbsoluteUri);
        public ActivityPubJsonBuilder PreferredUsername(string username) => SetKeyValue(JsonLdTypes.PREFERRED_USERNAME, username);
        public ActivityPubJsonBuilder AttributedTo(Uri uri) => SetKeyId(JsonLdTypes.ATTRIBUTED_TO, uri.AbsoluteUri);
        public ActivityPubJsonBuilder Content(string content) => SetKeyValue(JsonLdTypes.CONTENT, content);
        public ActivityPubJsonBuilder Cc(Uri uri) => SetKeyId(JsonLdTypes.CC, uri.AbsoluteUri);
        public ActivityPubJsonBuilder To(Uri uri) => SetKeyId(JsonLdTypes.TO, uri.AbsoluteUri);
        public ActivityPubJsonBuilder To(List<Uri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.TO) : SetKeyIds(JsonLdTypes.TO, uris.Select(uri => uri.AbsoluteUri));
        public ActivityPubJsonBuilder Cc(List<Uri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.CC) : SetKeyIds(JsonLdTypes.CC, uris.Select(uri => uri.AbsoluteUri));
        public ActivityPubJsonBuilder Bto(List<Uri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.BTO) : SetKeyIds(JsonLdTypes.BTO, uris.Select(uri => uri.AbsoluteUri));
        public ActivityPubJsonBuilder Bcc(List<Uri>? uris) => (uris == null || uris.Count == 0) ? ClearKey(JsonLdTypes.BCC) : SetKeyIds(JsonLdTypes.BCC, uris.Select(uri => uri.AbsoluteUri));
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
        public ActivityPubJsonBuilder Object(Uri uri) => SetKeyId(JsonLdTypes.OBJECT, uri.AbsoluteUri);
        public ActivityPubJsonBuilder Actor(Uri uri) => SetKeyId(JsonLdTypes.ACTOR, uri.AbsoluteUri);
        public JArray Build()
        {
            return _state;
        }
    }

}
