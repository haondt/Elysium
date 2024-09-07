using DotNext;
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
        private Result<JArray> _state = new([]);
        private static Func<JObject> JObjectFactory = () => new();
        private static Func<JArray> JArrayFactory = () => new();

        public ActivityPubJsonBuilder Type(string type)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                .Set("@type", new JArray { type });
            return this;
        }
        
        public ActivityPubJsonBuilder Id(Uri id)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                .Set("@id", new JArray { id.ToString() });
            return this;
        }
        private ActivityPubJsonBuilder SetKeyValue(string key, string value)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                .Set(key, new JArray { new JObject { { "@value", value } } });
            return this;
        }
        private ActivityPubJsonBuilder SetKeyId(string key, string value)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                .Set(key, new JArray { new JObject { { "@id", value } } });
            return this;
        }
        public ActivityPubJsonBuilder Inbox(Uri uri) => SetKeyId(JsonLdTypes.INBOX, uri.ToString());
        public ActivityPubJsonBuilder Outbox(Uri uri) => SetKeyId(JsonLdTypes.OUTBOX, uri.ToString());
        public ActivityPubJsonBuilder Followers(Uri uri) => SetKeyId(JsonLdTypes.FOLLOWERS, uri.ToString());
        public ActivityPubJsonBuilder Following(Uri uri) => SetKeyId(JsonLdTypes.FOLLOWING, uri.ToString());
        public ActivityPubJsonBuilder PreferredUsername(string username) => SetKeyValue(JsonLdTypes.PREFERRED_USERNAME, username.ToString());
        public ActivityPubJsonBuilder Content(string content) => SetKeyValue(JsonLdTypes.CONTENT, content);
        public ActivityPubJsonBuilder Cc(Uri uri) => SetKeyId(JsonLdTypes.CC, uri.ToString());
        public ActivityPubJsonBuilder To(Uri uri) => SetKeyId(JsonLdTypes.TO, uri.ToString());
        public ActivityPubJsonBuilder Published(DateTime dateTime)
        {
            _state.SetDefault(0, JObjectFactory, JObjectFactory)
                .Set(JsonLdTypes.PUBLISHED, new JArray
                {
                    new JObject
                    {
                        { "@type", JsonLdTypes.DATETIME },
                        { "@value", dateTime.AsXsdString() }
                    }
                });
            return this;
        }
        public Result<JArray> Build()
        {
            return _state;
        }
    }

}
