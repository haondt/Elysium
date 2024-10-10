using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.Cryptography.Services;
using Elysium.GrainInterfaces.Services;
using Newtonsoft.Json.Linq;
using Orleans.Concurrency;

namespace Elysium.GrainInterfaces.LocalActor
{
    /// <summary>
    /// Grain representation of a local actor
    /// </summary>
    ///<remarks><see href="https://www.w3.org/TR/activitypub/#actors"/></remarks> 
    public interface ILocalActorGrain : IGrain<LocalIri>
    {
        Task ClearAsync();


        Task IngestActivityAsync(Iri sender, ActivityType activityType, JToken activity);
        //Task<OrderedCollection> GetPublishedActivities(Optional<Actor> requester);

        /// <summary>
        /// Ask the grain to publish a new activity.
        /// The grain will do the following<br/>
        ///   - build an activity around the object<br/>
        ///     - e.g. if the activity type is create, it will assign an id to the object, and persist it on disk<br/>
        ///     - if it is delete, it will delete the object, etc<br/>
        ///   - persist the activity<br/>
        ///   - perform operations on the object<br/>
        ///   - publish the activity to its followers<br/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="object"></param>
        /// <returns>the <see cref="Iri"/> of the created activity</returns>
        Task<(LocalIri ActivityUri, JObject Activity)> PublishActivity(ActivityType type, JArray @object);

        /// <summary>
        /// Ask the grain to publish a transient activity.
        /// The grain will do the following<br/>
        ///   - build an activity around the object<br/>
        ///     - e.g. if the activity type is create, it will assing an id to the object, and persist it on disk<br/>
        ///     - if it is delete, it will delete the object, etc<br/>
        ///   - perform operations on the object<br/>
        ///   - publish the activity to its followers<br/>
        /// </summary>
        /// <remarks>the activity will not be persisted or assigned an id</remarks>
        /// <param name="type"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        Task PublishTransientActivity(ActivityType type, JObject @object);

        [AlwaysInterleave]
        Task<bool> IsInitializedAsync();
        [AlwaysInterleave]
        Task<LocalActorState> GetStateAsync();
        Task InitializeAsync(ActorRegistrationDetails registrationDetails);
        [AlwaysInterleave]
        Task<PlaintextCryptographicActorData> GetCryptographicDataAsync();
    }
}
