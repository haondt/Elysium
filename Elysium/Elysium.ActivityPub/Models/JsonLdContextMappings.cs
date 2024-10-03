using System.Diagnostics.CodeAnalysis;

namespace Elysium.ActivityPub.Models
{
    public static class JsonLdContextMappings
    {
        private static readonly Dictionary<string, string> _map = new()
        {
            { JsonLdTypes.INBOX, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.OUTBOX, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.FOLLOWERS, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.FOLLOWING, JsonLdContexts.ACTIVITY_STREAMS },

            // other actor fields
            { JsonLdTypes.PREFERRED_USERNAME, JsonLdContexts.ACTIVITY_STREAMS },

            // addressing
            { JsonLdTypes.BTO, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.TO, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.CC, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.BCC, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.AUDIENCE, JsonLdContexts.ACTIVITY_STREAMS },

            { JsonLdTypes.PUBLISHED, JsonLdContexts.ACTIVITY_STREAMS },

            // activity object types
            { JsonLdTypes.NOTE, JsonLdContexts.ACTIVITY_STREAMS },

            // activity object fields
            { JsonLdTypes.CONTENT, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.DATETIME, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.ATTRIBUTED_TO, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.OBJECT, JsonLdContexts.ACTIVITY_STREAMS },
            { JsonLdTypes.ACTOR, JsonLdContexts.ACTIVITY_STREAMS },

            // crypto
            { JsonLdTypes.PUBLIC_KEY, JsonLdContexts.SECURITY },
            { JsonLdTypes.PUBLIC_KEY_PEM, JsonLdContexts.SECURITY },
            { JsonLdTypes.ASSERTION_METHOD, JsonLdContexts.SECURITY },
            { JsonLdTypes.PUBLIC_KEY_MULTIBASE, JsonLdContexts.SECURITY },

            // activity types
            { JsonLdTypes.CREATE_ACTIVITY, JsonLdContexts.ACTIVITY_STREAMS },

            // actor types
            { JsonLdTypes.PERSON, JsonLdContexts.ACTIVITY_STREAMS }
        };

        public static bool TryGetContext(string type, [NotNullWhen(true)] out string? context)
        {
            // todo: maybe this should throw an error? maybe at startup not runtime to validate all the types are mapped
            return _map.TryGetValue(type, out context);
        }
    }
}
