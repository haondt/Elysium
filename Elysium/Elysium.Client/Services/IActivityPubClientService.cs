using DotNext;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{

    // implementation will be scoped, and use the session data to determine user identity
    public interface IActivityPubClientService
    {

        // uri = id of the object
        // existing id will be ignored
        public Task<Result<Uri>> CreateActivity(JObject @object);

        // object must have an id
        public Task<Optional<Exception>> UpdateActivity(JObject @object);

        // id = uri  of object to delete
        public Task<Optional<Exception>> DeleteActivity(Uri id);

        // id = person to follow
        public Task<Optional<Exception>> FollowActivity(Uri id);

        // dont forget to strip bcc and bto https://www.w3.org/TR/activitypub/#security-not-displaying-bto-bcc
        public Task<(Result<JObject> Document, DocumentRetrievalReason Reason)> RetrieveDocumentAsync(Uri id);

        you get the gist...



    }
}
