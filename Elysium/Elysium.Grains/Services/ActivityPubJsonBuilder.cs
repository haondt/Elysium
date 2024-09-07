using DotNext;
using Elysium.Grains.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
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
        public ActivityPubJsonBuilder Inbox(Uri uri) => SetKeyId("http://www.w3.org/ns/ldp#inbox", uri.ToString());
        public ActivityPubJsonBuilder Outbox(Uri uri) => SetKeyId("https://www.w3.org/ns/activitystreams#outbox", uri.ToString());
        public ActivityPubJsonBuilder Followers(Uri uri) => SetKeyId("https://www.w3.org/ns/activitystreams#followers", uri.ToString());
        public ActivityPubJsonBuilder Following(Uri uri) => SetKeyId("https://www.w3.org/ns/activitystreams#following", uri.ToString());
        public ActivityPubJsonBuilder PreferredUsername(string username) => SetKeyValue("https://www.w3.org/ns/activitystreams#preferredUsername", username.ToString());
        public Result<JArray> Build()
        {
            return _state;
        }
    }

}
