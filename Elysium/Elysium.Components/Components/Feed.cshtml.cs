
using Elysium.Client.Services;
using Elysium.Components.Abstractions;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class FeedModel : IComponentModel
    {
        public required List<IComponent<MediaModel>> Media { get; set; }

        public static IComponentDescriptor Register(IServiceProvider serviceProvider)
        {
            return new ComponentDescriptor<FeedModel>(async cf =>
            {
                var elysiumService = serviceProvider.GetRequiredService<IElysiumService>();
                var creations = await elysiumService.GetPublicCreations();
                var mediaModels = creations.Creations
                    .Select(m => new MediaModel
                    {
                        AuthorFediverseHandle = m.AuthorFediverseHandle,
                        AuthorName = m.AuthorName,
                        Depth = m.Depth,
                        NumReplies = m.NumReplies,
                        Text = m.Text,
                        Timestamp = m.Timestamp,
                        Title = m.Title,
                    })
                    .ToList();
                var mediaComponents = await Task.WhenAll(mediaModels.Select(c => cf.GetComponent(c)));

                return new FeedModel
                {
                    Media = mediaComponents.ToList()
                };
            })
            {
                ViewPath = "~/Components/Feed.cshtml",
            };
        }
    }

    public class FeedComponentDescriptorFactory(IElysiumService elysiumService) : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new ComponentDescriptor<FeedModel>(async cf =>
            {
                var creations = await elysiumService.GetPublicCreations();
                var mediaModels = creations.Creations
                    .Select(m => new MediaModel
                    {
                        AuthorFediverseHandle = m.AuthorFediverseHandle,
                        AuthorName = m.AuthorName,
                        Depth = m.Depth,
                        NumReplies = m.NumReplies,
                        Text = m.Text,
                        Timestamp = m.Timestamp,
                        Title = m.Title,
                    })
                    .ToList();
                var mediaComponents = await Task.WhenAll(mediaModels.Select(c => cf.GetComponent(c)));

                return new FeedModel
                {
                    Media = mediaComponents.ToList()
                };
            })
            {
                ViewPath = "~/Components/Feed.cshtml",
            };
        }
    }
}
