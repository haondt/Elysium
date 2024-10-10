using Elysium.Authentication.Components;
using Elysium.Client.Services;
using Elysium.Components.Abstractions;
using Elysium.Components.Exceptions;
using Haondt.Web.Core.Components;

namespace Elysium.Components.Components
{
    public class ShadeSelectorModel : IComponentModel
    {
        public required List<ShadeSelection> ShadeSelections { get; set; }
    }
    public class ShadeSelection
    {
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public bool HasNotifications { get; set; }
        public required string Text { get; set; }
    }

    public class ShadeSelectorComponentDescriptorFactory(
        IUserSessionService session,
        IElysiumService elysiumService) : IComponentDescriptorFactory
    {
        public IComponentDescriptor Create()
        {
            return new NeedsAuthorizationComponentDescriptor<ShadeSelectorModel>(async (cf, rd) =>
            {
                var username = await session.GetLocalizedUsernameAsync();
                if (!username.IsSuccessful)
                    throw new ComponentException($"Failed to retrieve username. reason: {username.Reason}");
                var iri = await session.GetIriAsync();
                if (!iri.IsSuccessful)
                    throw new ComponentException($"Failed to retrieve iri. reason: {iri.Reason}");
                var shades = await session.GetShadesAsync();
                if (!shades.IsSuccessful)
                    throw new ComponentException($"Failed to load user shades. reason: {shades.Reason}");
                var activeShadeOptional = session.GetActiveShade();
                var activeShade = activeShadeOptional.HasValue ? activeShadeOptional.Value : iri.Value;

                var model = new ShadeSelectorModel
                {
                    ShadeSelections = new List<ShadeSelection>
                    {
                        new ()
                        {
                            IsPrimary = true,
                            Text = $"@{username.Value}",
                            IsActive = activeShade == iri.Value
                        }
                    }
                };

                foreach (var shade in shades.Value)
                {
                    model.ShadeSelections.Add(new ShadeSelection
                    {
                        IsActive = activeShade == shade,
                        IsPrimary = false,
                        Text = elysiumService.GetShadeNameFromLocalIri(iri.Value, shade)
                    });
                }

                return model;
            })
            {
                ViewPath = "~/Components/ShadeSelector.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated]
            };
        }
    }
}
