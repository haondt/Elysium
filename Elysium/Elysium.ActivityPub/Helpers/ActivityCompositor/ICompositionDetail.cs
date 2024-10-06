namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public interface ICompositionDetail
    {
        public ActivityPubJsonBuilder Apply(ActivityPubJsonBuilder builder);
    }
}
