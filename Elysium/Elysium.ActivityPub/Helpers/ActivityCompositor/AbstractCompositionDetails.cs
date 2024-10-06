using Newtonsoft.Json.Linq;

namespace Elysium.ActivityPub.Helpers.ActivityCompositor
{
    public abstract class AbstractCompositionDetails : ICompositionDetails
    {
        public abstract string Type { get; }
        protected abstract List<ICompositionDetail> AggregateDetails();
        protected virtual ActivityPubJsonBuilder AdditionalConfiguration(ActivityPubJsonBuilder builder) => builder;

        public JArray Composit()
        {
            var builder = new ActivityPubJsonBuilder()
                .Type(Type)
                .Published(DateTime.UtcNow);
            foreach (var detail in AggregateDetails())
                builder = detail.Apply(builder);
            builder = AdditionalConfiguration(builder);
            return builder.Build();
        }
    }
}
