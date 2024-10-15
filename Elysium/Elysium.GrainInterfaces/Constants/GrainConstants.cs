namespace Elysium.GrainInterfaces.Constants
{
    public class GrainConstants
    {
        // persistent state storages
        public const string GrainDocumentStorage = "Documents";
        public const string GrainStorage = "Grains";

        // queues
        public const string RemoteActorOutgoingDataQueue = "RemoteActorOutgoingDataQueue";
        public const string RemoteActorIncomingDataQueue = "RemoteActorIncomingDataQueue";
        public const string LocalActorOutgoingProcessingQueue = "LocalActorOutgoingProcessingQueue";
        public const string LocalActorIncomingProcessingQueue = "LocalActorIncomingProcessingQueue";
    }
}
