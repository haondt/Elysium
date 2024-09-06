using DotNext;
using Elysium.GrainInterfaces;
using KristofferStrube.ActivityStreams;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Services
{
    public interface IActivityPubService
    {
        /// <summary>
        /// Servers performing delivery to the inbox or sharedInbox properties of actors on other servers MUST
        /// provide the object property in the activity: Create, Update, Delete, Follow, Add, Remove, Like, 
        /// Block, Undo. Additionally, servers performing server to server delivery of the following activities 
        /// MUST also provide the target property: Add, Remove.
        /// </summary>
        /// <remarks><see href="https://www.w3.org/TR/activitypub/#server-to-server-interactions"/></remarks>
        /// <param name="sender"></param>
        /// <param name="recepient"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<Optional<Exception>> PublishActivityAsync(Uri sender, Uri recepient, JObject activity);
        Task<Optional<Exception>> IngestRemoteActivityAsync(IncomingRemoteActivityData data);
    }
}
