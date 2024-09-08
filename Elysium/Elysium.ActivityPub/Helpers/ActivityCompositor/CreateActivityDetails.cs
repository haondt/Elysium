using Elysium.ActivityPub.Helpers.ActivityCompositor;
using Elysium.ActivityPub.Models;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public class CreateActivityDetails : ActivityDetails
    {
        public override string Type => JsonLdTypes.CREATE_ACTIVITY;
    }
}
