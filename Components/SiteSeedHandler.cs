using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace champion_4_heroes.Components;

/// <summary>
/// Creates the "home" document type (with every property used by Views/Home.cshtml) and the first
/// published content item once Umbraco has started. Both steps only run when the item is missing,
/// so edits made later in the back office are never overwritten.
/// </summary>
public sealed class SiteSeedHandler : INotificationHandler<UmbracoApplicationStartedNotification>
{
    private const string ContentTypeAlias = "home";
    private const string TemplateAlias = "Home";

    /// <summary>
    /// Group alias, group name, property alias, property name, data type id, seeded value.
    /// </summary>
    private sealed record Field(string Group, string GroupName, string Alias, string Name, int DataTypeId, string Value);

    private static readonly Field[] Fields =
    {
        // Hero
        new("hero", "Hero", "heroEyebrow", "Eyebrow", Constants.DataTypes.Textbox,
            "A program of the Michael Nathan Twining Foundation"),
        new("hero", "Hero", "heroTitle", "Title", Constants.DataTypes.Textbox, "Champions 4 Heroes"),
        new("hero", "Hero", "heroLead", "Lead paragraph", Constants.DataTypes.Textarea,
            "We unite veterans and boxing champions to advocate for, support, and educate on the effects of " +
            "traumatic brain injury, keeping alive the memory of Army Staff Sergeant Michael \"Nate\" Twining " +
            "of the 101st Airborne."),

        // Mission
        new("mission", "Mission", "missionTitle", "Section title", Constants.DataTypes.Textbox,
            "Fighting for the people who fought for us"),
        new("mission", "Mission", "missionLead", "Section lead", Constants.DataTypes.Textarea,
            "Founded by veteran and former boxer Mel Twining, Champions 4 Heroes brings the discipline of the ring " +
            "and the bond of the military family together for one purpose: veterans living with traumatic brain " +
            "injury and their families should never fight alone."),
        new("mission", "Mission", "pillar1Title", "Pillar 1 title", Constants.DataTypes.Textbox, "Advocacy"),
        new("mission", "Mission", "pillar1Text", "Pillar 1 text", Constants.DataTypes.Textarea,
            "We speak up for veterans dealing with the long-term effects of blast exposure and combat injuries, " +
            "pushing for recognition, care, and the resources they have earned."),
        new("mission", "Mission", "pillar2Title", "Pillar 2 title", Constants.DataTypes.Textbox, "Resources"),
        new("mission", "Mission", "pillar2Text", "Pillar 2 text", Constants.DataTypes.Textarea,
            "We connect veterans and families with the organizations that changed our own lives, including Fisher " +
            "House and Friends of Fisher House Illinois, plus a network of champions who show up."),
        new("mission", "Mission", "pillar3Title", "Pillar 3 title", Constants.DataTypes.Textbox, "Education"),
        new("mission", "Mission", "pillar3Text", "Pillar 3 text", Constants.DataTypes.Textarea,
            "We teach communities, gyms, and families about traumatic brain injury: its signs, its effects, and why " +
            "early support can change the entire course of a life."),

        // Story
        new("story", "Nate's Story", "storyTitle", "Section title", Constants.DataTypes.Textbox,
            "Why this foundation exists"),
        new("story", "Nate's Story", "storyText1", "Paragraph 1", Constants.DataTypes.Textarea,
            "Staff Sergeant Michael \"Nate\" Twining spent his career as a 101st Airborne soldier, with a combat tour " +
            "in Iraq and another in Afghanistan. It wasn't a bullet or an IED that took him. It was a brain tumor, " +
            "the result of exposure he encountered during those deployments."),
        new("story", "Nate's Story", "storyText2", "Paragraph 2", Constants.DataTypes.Textarea,
            "For two years and one week, Nate and his father Mel lived at the Chicago VA Fisher House while he " +
            "received treatment. In that time Nate touched everyone who knew him. When Nate died, Mel turned grief " +
            "into a mission: honor his son by serving the veterans who come home carrying invisible wounds."),
        new("story", "Nate's Story", "quote", "Pull quote", Constants.DataTypes.Textarea,
            "While we were living here, Nate touched everyone, and everyone loved Nate. Just ask anyone who knew my son."),
        new("story", "Nate's Story", "quoteAuthor", "Quote attribution", Constants.DataTypes.Textbox,
            "Mel Twining, at the Chicago VA Fisher House"),

        // Programs
        new("programs", "What We Do", "programsTitle", "Section title", Constants.DataTypes.Textbox,
            "The ring, the gym, and the community"),
        new("programs", "What We Do", "programsLead", "Section lead", Constants.DataTypes.Textarea,
            "Boxing taught us resilience. Service taught us loyalty. We put both to work for veterans and the " +
            "families standing beside them."),
        new("programs", "What We Do", "program1Title", "Program 1 title", Constants.DataTypes.Textbox,
            "Champions & Veterans Together"),
        new("programs", "What We Do", "program1Text", "Program 1 text", Constants.DataTypes.Textarea,
            "Exhibition bouts, gym sessions, and mentorship that put boxing champions side by side with veterans, " +
            "building discipline, camaraderie, and a reason to keep showing up."),
        new("programs", "What We Do", "program2Title", "Program 2 title", Constants.DataTypes.Textbox,
            "TBI Awareness & Advocacy"),
        new("programs", "What We Do", "program2Text", "Program 2 text", Constants.DataTypes.Textarea,
            "Talks, outreach, and campaigns that explain traumatic brain injury in plain language: what blast " +
            "exposure does, what to watch for, and where to turn for care."),
        new("programs", "What We Do", "program3Title", "Program 3 title", Constants.DataTypes.Textbox,
            "Family Support & Resources"),
        new("programs", "What We Do", "program3Text", "Program 3 text", Constants.DataTypes.Textarea,
            "Connecting families with housing, support networks, and organizations like Fisher House, where love and " +
            "community carry a family through the hardest days."),
        new("programs", "What We Do", "program4Title", "Program 4 title", Constants.DataTypes.Textbox,
            "Events & Fundraising"),
        new("programs", "What We Do", "program4Text", "Program 4 text", Constants.DataTypes.Textarea,
            "Community events and fundraisers that keep Nate's memory alive and fund the programs veterans rely on. " +
            "Every ticket, glove, and dollar has a name attached to it."),

        // Watch
        new("watch", "Watch & Share", "watchTitle", "Section title", Constants.DataTypes.Textbox,
            "See the story for yourself"),
        new("watch", "Watch & Share", "watchLead", "Section lead", Constants.DataTypes.Textarea,
            "Share these films with a veteran, a gym, or a family that needs to know they are not alone."),
        new("watch", "Watch & Share", "watchNote", "Closing note", Constants.DataTypes.Textarea,
            "Today we remember and celebrate the life of Nate Twining. For more than two years, Mel and Nate stayed " +
            "at Fisher House while Nate received treatment, and today, Mel keeps his son's memory alive through his " +
            "own foundation."),
        new("watch", "Watch & Share", "youtubeUrl", "YouTube video URL", Constants.DataTypes.Textbox,
            "https://www.youtube.com/watch?v=fmotPKW5c-s"),
        new("watch", "Watch & Share", "facebookVideoUrl", "Facebook video URL", Constants.DataTypes.Textbox,
            "https://www.facebook.com/watch/?v=365086695465717"),

        // Get involved
        new("involved", "Get Involved", "involvedTitle", "Section title", Constants.DataTypes.Textbox,
            "Every champion starts with one round"),
        new("involved", "Get Involved", "involvedLead", "Section lead", Constants.DataTypes.Textarea,
            "Whether you lace up, write a check, or simply share the story, there is a place for you here."),
        new("involved", "Get Involved", "ctaHeading", "Banner heading", Constants.DataTypes.Textbox,
            "For Nate. For every veteran. For every hero."),

        // Contact & footer
        new("contact", "Contact & Footer", "contactIntro", "Contact intro", Constants.DataTypes.Textarea,
            "The fastest way to reach us is through Facebook, which is where our community gathers, shares events, " +
            "and keeps Nate's story alive."),
        new("contact", "Contact & Footer", "facebookUrl", "Facebook profile URL", Constants.DataTypes.Textbox,
            "https://www.facebook.com/nate.twining.733/"),
        new("contact", "Contact & Footer", "address", "Mailing address", Constants.DataTypes.Textbox,
            "Michael Nathan Twining Foundation, 8212 S 116th Ave, Rothbury, MI 49452"),
        new("contact", "Contact & Footer", "footerText", "Footer description", Constants.DataTypes.Textarea,
            "A program of the Michael Nathan Twining Foundation, a 501(c)(3) nonprofit honoring SSG Michael " +
            "\"Nate\" Twining, U.S. Army, 101st Airborne Division."),
    };

    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;
    private readonly IDataTypeService _dataTypeService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly ILogger<SiteSeedHandler> _logger;

    public SiteSeedHandler(
        IContentTypeService contentTypeService,
        IContentService contentService,
        IDataTypeService dataTypeService,
        IShortStringHelper shortStringHelper,
        ILogger<SiteSeedHandler> logger)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _dataTypeService = dataTypeService;
        _shortStringHelper = shortStringHelper;
        _logger = logger;
    }

    public void Handle(UmbracoApplicationStartedNotification notification)
    {
        try
        {
            Seed();
        }
        catch (Exception ex)
        {
            // Never take the site down over seeding (for example a database still being installed).
            _logger.LogError(ex, "Champions 4 Heroes: could not provision the document type or content.");
        }
    }

    private void Seed()
    {
        IContentType contentType = _contentTypeService.Get(ContentTypeAlias) as IContentType
            ?? CreateContentType();

        IContent? home = _contentService.GetRootContent().FirstOrDefault();

        if (home is null)
        {
            home = _contentService.Create("Home", Constants.System.Root, contentType);

            foreach (Field field in Fields)
            {
                home.SetValue(field.Alias, field.Value);
            }

            _contentService.Save(home);
            _logger.LogInformation("Champions 4 Heroes: created the home page content item.");
        }

        if (home.Published)
        {
            return;
        }

        // Publishing requires an explicit culture list; invariant content publishes without cultures.
        var result = _contentService.Publish(home, Array.Empty<string>());

        _logger.LogInformation(
            "Champions 4 Heroes: published the home page (published: {Published}, result: {Result}).",
            home.Published,
            result.Result);
    }

    private IContentType CreateContentType()
    {
        var contentType = new ContentType(_shortStringHelper, Constants.System.Root)
        {
            Alias = ContentTypeAlias,
            Name = "Home",
            Description = "Champions 4 Heroes landing page",
            Icon = "icon-home color-orange",
            AllowedAsRoot = true,
        };

        foreach (Field field in Fields)
        {
            IDataType dataType = GetDataType(field.DataTypeId);

            var propertyType = new PropertyType(_shortStringHelper, dataType)
            {
                Alias = field.Alias,
                Name = field.Name,
            };

            contentType.AddPropertyType(propertyType, field.Group, field.GroupName);
        }

        var created = _contentTypeService
            .CreateAsync(contentType, Constants.Security.SuperUserKey)
            .GetAwaiter()
            .GetResult();

        if (!created.Success)
        {
            throw new InvalidOperationException(
                $"Could not create the '{ContentTypeAlias}' document type: {created.Result}");
        }

        // Reload to pick up the persisted key before attaching the template.
        IContentType saved = _contentTypeService.Get(ContentTypeAlias) as IContentType ?? contentType;

        // Creates the template and marks it as the default template for this document type,
        // which is what routes "/" to Views/Home.cshtml.
        _contentTypeService
            .CreateTemplateAsync(saved.Key, "Home", TemplateAlias, true, Constants.Security.SuperUserKey)
            .GetAwaiter()
            .GetResult();

        _logger.LogInformation(
            "Champions 4 Heroes: created document type '{Alias}' with {Count} properties.",
            saved.Alias,
            saved.PropertyTypes.Count());

        // Reload again so the saved template association is picked up.
        return _contentTypeService.Get(ContentTypeAlias) as IContentType ?? saved;
    }

    /// <summary>
    /// Resolves a built-in data type (-88 textbox / -89 textarea) from the installed Umbraco data types.
    /// </summary>
    private IDataType GetDataType(int builtInId)
    {
        string editorAlias = builtInId == Constants.DataTypes.Textarea
            ? "Umbraco.Textarea"
            : "Umbraco.TextBox";

        IEnumerable<IDataType> matches =
            _dataTypeService.GetByEditorAliasAsync(editorAlias).GetAwaiter().GetResult();

        return matches.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"No data type using the '{editorAlias}' editor was found; the document type cannot be created.");
    }
}
