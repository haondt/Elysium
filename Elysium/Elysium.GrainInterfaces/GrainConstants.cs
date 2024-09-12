using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces
{
    public class GrainConstants
    {
        // persistent state storages
        public const string GrainDocumentStorage = "Documents";
        public const string GrainStorage = "Grains";

        // stream providers
        public const string SimpleStreamProvider = "SimpleStreamProvider";

        // streams
        public const string DispatchRemoteActivityStream = "DispatchRemoteActivityStream";
        public const string LocalActorOutgoingProcessingStream = "LocalActorOutgoingProcessingStream";
        public const string LocalActorIncomingProcessingStream = "LocalActorIncomingProcessingStream";
    }
}
