namespace Elysium.ActivityPub.Models
{
    public enum ActivityType
    {
        Unknown = 0,
        Create,
        Update,
        Delete,
        Follow,
        Add,
        Remove,
        Like,
        Block,
        Undo
    }
}
