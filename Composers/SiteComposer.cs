using champion_4_heroes.Components;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace champion_4_heroes.Composers;

/// <summary>
/// Registers the handler that provisions the "home" document type and seeds the first page,
/// so a freshly installed site renders without manual back office setup.
/// </summary>
public sealed class SiteComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder) =>
        builder.AddNotificationHandler<UmbracoApplicationStartedNotification, SiteSeedHandler>();
}
