using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Haondt.Identity.StorageKey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Client.Services
{

    public interface IActivityPubClientService
    {
        Task<LocalIri> GetLocalIriFromUserIdentityAsync(StorageKey<UserIdentity> identity);

        // Todo: method that parses @foo@baz.bar and turns it into https://baz.bar/x/y/z/foo
        //Task<Result<>>

        // iri = id of the object
        // existing id will be ignored
        Task<(LocalIri ActivityUri, LocalIri ObjectUri)> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, JArray @object);
        //Task<Result<LocalUri>> PublishActivityAsync(StorageKey<UserIdentity> author, ActivityType type, RemoteUri @object);

        // object must have an id
        //public Task<Optional<Exception>> UpdateActivity(JObject @object);

        //// id = iri  of object to delete
        //public Task<Optional<Exception>> DeleteActivity(Iri id);

        //// id = person to follow
        //public Task<Optional<Exception>> FollowActivity(Iri id);

        //// dont forget to strip bcc and bto https://www.w3.org/TR/activitypub/#security-not-displaying-bto-bcc
        //public Task<(Result<JObject> Document, DocumentRetrievalReason Reason)> RetrieveDocumentAsync(Iri id);
    }
}
