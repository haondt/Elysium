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
