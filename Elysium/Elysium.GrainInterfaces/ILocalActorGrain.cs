using DotNext;
using Elysium.ActivityPub.Models;
using Elysium.GrainInterfaces.Services;
using Elysium.Hosting.Models;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    /// <summary>
    /// Grain representation of a local actor
    /// </summary>
    ///<remarks><see href="https://www.w3.org/TR/activitypub/#actors"/></remarks> 
    public interface ILocalActorGrain : IGrain<LocalUri>
    {
        Task<Optional<Exception>> InitializeDocument();


        Task<Optional<Exception>> IngestActivityAsync(JObject activity);
        //Task<OrderedCollection> GetPublishedActivities(Optional<Actor> requester);
        Task<Result<byte[]>> GetSigningKeyAsync();

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
        /// <returns>the <see cref="Uri"/> of the created activity</returns>
        Task<Result<Uri>> PublishActivity(ActivityType type, JObject @object);

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
        Task<Optional<Exception>> PublishTransientActivity(ActivityType type, JObject @object);


    }
}
