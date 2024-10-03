using Elysium.ActivityPub.Models;
using Elysium.Core.Models;

namespace Elysium.GrainInterfaces
{
    public interface IPublicCollectionGrain : IGrainWithGuidKey
    {
        Task<CollectionResult> GetReferencesAsync(ActivityType activityType, int count);
        Task<CollectionResult> GetReferencesAsync(ActivityType activitytype, long before, int count);
        Task IngestReferenceAsync(ActivityType activityType, Iri iri);
    }
}
