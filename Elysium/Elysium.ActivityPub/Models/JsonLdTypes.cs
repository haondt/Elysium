using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.ActivityPub.Models
{
    public static class JsonLdTypes
    {
        // required actor fields
        public const string INBOX = "http://www.w3.org/ns/ldp#inbox";
        public const string OUTBOX = "https://www.w3.org/ns/activitystreams#outbox";
        public const string FOLLOWERS = "https://www.w3.org/ns/activitystreams#followers";
        public const string FOLLOWING = "https://www.w3.org/ns/activitystreams#following";

        // other actor fields
        public const string PREFERRED_USERNAME = "https://www.w3.org/ns/activitystreams#preferredUsername";

        // activity fields
        public const string CC = "https://www.w3.org/ns/activitystreams#cc";
        public const string TO = "https://www.w3.org/ns/activitystreams#to";
        public const string PUBLISHED = "https://www.w3.org/ns/activitystreams#published";

        // activity object types
        public const string NOTE = "https://www.w3.org/ns/activitystreams#Note";


        // activity object fields
        public const string CONTENT = "https://www.w3.org/ns/activitystreams#content";
        public const string DATETIME = "http://www.w3.org/2001/XMLSchema#dateTime";

        // crypto
        public const string PUBLIC_KEY = "https://w3id.org/security#publicKey";
        public const string PUBLIC_KEY_PEM = "https://w3id.org/security#publicKeyPem";
        public const string ASSERTION_METHOD = "https://w3id.org/security#assertionMethod";
        public const string PUBLIC_KEY_MULTIBASE = "https://w3id.org/security#publicKeyMultibase";
    }
}
