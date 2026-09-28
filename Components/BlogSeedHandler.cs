using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace champion_4_heroes.Components;

/// <summary>
/// Creates the "blogPost" and "blogListing" document types and seeds the Community &amp; News section
/// with posts sourced from the organization's Facebook posts. Like the home page seed, each step only
/// runs when the item is missing, so back office edits are never overwritten.
/// </summary>
public sealed class BlogSeedHandler : INotificationHandler<UmbracoApplicationStartedNotification>
{
    private const string PostTypeAlias = "blogPost";
    private const string ListingTypeAlias = "blogListing";

    /// <summary>Node name / URL segment for the listing page.</summary>
    private const string ListingName = "blog";

    /// <summary>Url segment, name, author, event tag, body, image file (null when Facebook blocked it), alt text, source url.</summary>
    private sealed record Post(
        string UrlSegment,
        string Name,
        string Author,
        string EventTag,
        string Body,
        string? Image,
        string ImageAlt,
        string SourceUrl);

    private static readonly Post[] Posts =
    {
        new(
            "wbc-and-national-boxing-hall-of-fame",
            "Champions 4 Heroes at the WBC and National Boxing Hall of Fame",
            "Eva D. Jones-Young",
            "National Boxing Hall of Fame",
            "CHAMPIONS 4 HEROS need I say more!!! LETS GO!!! We are out to help our veterans, athletes and " +
            "their caregivers!!!",
            "wbc-national-boxing-hall-of-fame.jpg",
            "Champions 4 Heroes supporters with friends in front of a WBC and National Boxing Hall of Fame backdrop",
            "https://www.facebook.com/share/p/1MW7GY6Kte/"),

        new(
            "international-boxing-hall-of-fame-canastota",
            "Road to Canastota: International Boxing Hall of Fame Weekend",
            "Eva D. Jones-Young",
            "International Boxing Hall of Fame",
            "We are on our way to Canastota, NY for the International Boxing Hall of Fame. We will meet up with " +
            "our powerful team members from Champions 4 Heroes. See everyone tomorrow at the Celebrity Fist " +
            "Casting!",
            "trilogy.jpg",
            string.Empty,
            "https://www.facebook.com/share/p/19kwttHumJ/"),

        new(
            "on-our-way-to-acbhof",
            "On Our Way to the Atlantic City Boxing Hall of Fame",
            "Janie Bracero",
            "Atlantic City Boxing Hall of Fame",
            "Champions 4 Heroes on the way to ACBHOF for a spectacular weekend to remember!!",
            "usteam.jpg",
            string.Empty,
            "https://www.facebook.com/share/p/1JpLEWjHGL/"),

        new(
            "macho-time-hall-of-fame-weekend",
            "Macho Time: Hall of Fame Weekend",
            "Hector Camacho Jr.",
            "Hall of Fame",
            "This weekend it will be Macho time! Champions 4 Heroes, Hall of Fame weekend. Special thanks to " +
            "Nate Twining, Jay Torres and the crew.",
            "macho-time-hall-of-fame-weekend.jpg",
            "Poster artwork for a Champions 4 Heroes Hall of Fame weekend",
            "https://www.facebook.com/share/p/1DsqPQuYjL/"),

        new(
            "thank-you-janie",
            "Thank You, Janie",
            "Eva D. Jones-Young",
            "Community",
            "Thank you Miss Janie for all the beautiful pictures and taking care of me at our home in " +
            "California. I am so glad you came and support me and Champions 4 Heroes. We love you.",
            "janie-bracero-thank-you.jpg",
            "A Champions 4 Heroes supporter thanking Janie Bracero for her support",
            "https://www.facebook.com/share/p/1EN9kEtbyb/"),

        new(
            "indiana-sports-hall-of-fame",
            "Champions 4 Heroes at the Indiana Sports Hall of Fame",
            "Eva D. Jones-Young",
            "Indiana Sports Hall of Fame",
            "Champions 4 Heroes at the Indiana Sports Hall of Fame! Larry and I love supporting such a great " +
            "not-for-profit organization that helps our military men and women, and our athletes with " +
            "traumatic brain injury.",
            "indiana-sports-hall-of-fame.jpg",
            "Champions 4 Heroes representatives at the Indiana Sports Hall of Fame",
            "https://www.facebook.com/share/p/1LiBjuVSPF/"),

        new(
            "nbhof-late-upload",
            "NBHOF Photo Drop: Late Uploads from the National Boxing Hall of Fame",
            "Ritchel N Twining",
            "National Boxing Hall of Fame",
            "Some photos from the National Boxing Hall of Fame, uploaded late.",
            "mosley.jpg",
            string.Empty,
            "https://www.facebook.com/share/r/1EaJCDe1d9/"),

        new(
            "brenda-spinks-hall-of-fame-award",
            "Congratulations to Brenda Spinks",
            "Eva D. Jones-Young",
            "Women's Boxing",
            "Mel Twining from Champions 4 Heroes pictured here with beautiful Brenda Spinks, wife of the late " +
            "great Leon Spinks. Congratulations Brenda on your award from the International Women's Boxing " +
            "Hall of Fame!",
            "brenda-spinks-award.jpg",
            "Mel Twining, wearing a Champions 4 Heroes t-shirt, photographed with Brenda Spinks at the " +
            "International Women's Boxing Hall of Fame",
            "https://www.facebook.com/share/p/1HYTr6QsFZ/"),

        new(
            "the-spinks-family-and-champions-4-heroes",
            "The Spinks Family and Champions 4 Heroes",
            "Brenda Glur Spinks",
            "Community",
            "With friends Mel and his beautiful wife Chel (Mel Twining), and Michael Spinks too! Mel runs the " +
            "Nate Twining Foundation, Champions 4 Heroes, which does great things to recognize and help veterans.",
            "bernardhopkins.jpg",
            string.Empty,
            "https://www.facebook.com/share/p/1d4qqrvBkC/"),

        new(
            "what-champions-4-heroes-stands-for",
            "What Champions 4 Heroes Stands For",
            "Janie Bracero",
            "Our Mission",
            "Champions 4 Heroes is a non-profit organization uniting veterans and boxing champions to promote " +
            "advocacy, provide resources, and educate the community on the effects of traumatic brain injury.",
            "completeteam.jpg",
            string.Empty,
            "https://www.facebook.com/share/p/1BqGKk9ChN/"),

        new(
            "champions-gift-to-hines-va",
            "Champions Gift to the VA at Hines",
            "Hines VA Hospital",
            "Veterans",
            "We received a visit today from the Dynasty CheerAbilities ICE All Stars, who recently took gold at " +
            "the 2022 USA Special Abilities Unified Cheer ICU. The champion cheerleaders gifted Veterans and " +
            "staff with a Champion gift.",
            "hines-va-champions-gift.jpg",
            "Veterans and staff at Hines VA Hospital receive a gift from champion cheerleaders",
            "https://www.facebook.com/share/p/1DpFgtgyan/"),

        new(
            "heros-welcome-in-bacolod",
            "Hero's Welcome in Bacolod",
            "Bacolod City PIO",
            "Boxing",
            "Newly crowned WBO super flyweight champion Donnie \"Ahas\" Nietes receives a hero's welcome in " +
            "Bacolod, with local officials joining him in a celebration of his achievement.",
            "bacolod-hero-welcome-nietes.jpg",
            "WBO super flyweight champion Donnie Nietes is welcomed in Bacolod",
            "https://www.facebook.com/share/p/1EhtBVrcsB/"),
    };

    private static readonly (string Group, string GroupName, string Alias, string Name, int DataTypeId)[] PostFields =
    {
        ("content", "Post", "postTitle", "Title", Constants.DataTypes.Textbox),
        ("content", "Post", "postAuthor", "Posted by", Constants.DataTypes.Textbox),
        ("content", "Post", "eventTag", "Event", Constants.DataTypes.Textbox),
        ("content", "Post", "postBody", "Body", Constants.DataTypes.Textarea),
        ("content", "Post", "imageFile", "Image file (wwwroot/img/blog)", Constants.DataTypes.Textbox),
        ("content", "Post", "imageAlt", "Image alt text", Constants.DataTypes.Textbox),
        ("content", "Post", "sourceUrl", "Original Facebook post URL", Constants.DataTypes.Textbox),
    };

    private static readonly (string Group, string GroupName, string Alias, string Name, int DataTypeId)[] ListingFields =
    {
        ("content", "Page", "listingTitle", "Title", Constants.DataTypes.Textbox),
        ("content", "Page", "listingLead", "Lead paragraph", Constants.DataTypes.Textarea),
    };

    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;
    private readonly IEntityService _entityService;
    private readonly IPublishedContentCache _publishedCache;
    private readonly IDataTypeService _dataTypeService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly ILogger<BlogSeedHandler> _logger;

    public BlogSeedHandler(
        IContentTypeService contentTypeService,
        IContentService contentService,
        IEntityService entityService,
        IPublishedContentCache publishedCache,
        IDataTypeService dataTypeService,
        IShortStringHelper shortStringHelper,
        ILogger<BlogSeedHandler> logger)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _entityService = entityService;
        _publishedCache = publishedCache;
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
            _logger.LogError(ex, "Champions 4 Heroes: could not provision the blog document types or content.");
        }
    }

    private void Seed()
    {
        IContentType postType = _contentTypeService.Get(PostTypeAlias) as IContentType
            ?? CreateContentType(PostTypeAlias, "Blog Post", PostFields);

        IContentType listingType = _contentTypeService.Get(ListingTypeAlias) as IContentType
            ?? CreateContentType(ListingTypeAlias, "Blog Listing", ListingFields, allowedAsRoot: false);

        IContent? home = _contentService.GetRootContent().FirstOrDefault();
        if (home is null)
        {
            _logger.LogWarning("Champions 4 Heroes: no root content yet, skipping blog seed.");
            return;
        }

        // The listing lives at /blog; its node name becomes the URL segment.
        IContent? existingListing = Children(home.Id)
            .FirstOrDefault(c => c.ContentType.Alias == ListingTypeAlias);

        IContent listing;
        if (existingListing is null)
        {
            listing = _contentService.Create(ListingName, home.Id, listingType);
            listing.SetValue("listingTitle", "Community & News");
            listing.SetValue(
                "listingLead",
                "Champions 4 Heroes in the ring, at the hall of fame ceremonies, and alongside the veterans we serve.");
            _contentService.Save(listing);
            _contentService.Publish(listing, Array.Empty<string>());
        }
        else
        {
            listing = existingListing;
        }

        var existingPosts = Children(listing.Id)
            .Where(c => c.ContentType.Alias == PostTypeAlias)
            .ToDictionary(c => c.Name ?? string.Empty, c => c, StringComparer.OrdinalIgnoreCase);

        int created = 0;
        int backfilled = 0;

        foreach (Post post in Posts)
        {
            // The post node name is its URL segment, so dedupe on that.
            if (existingPosts.TryGetValue(post.UrlSegment, out IContent? existing))
            {
                if (AttachImage(existing, post))
                {
                    backfilled++;
                }

                continue;
            }

            // The post node name doubles as its URL segment, so keep it url-friendly.
            IContent item = _contentService.Create(post.UrlSegment, listing.Id, postType);
            item.SetValue("postTitle", post.Name);
            item.SetValue("postAuthor", post.Author);
            item.SetValue("eventTag", post.EventTag);
            item.SetValue("postBody", post.Body);
            item.SetValue("imageFile", post.Image ?? string.Empty);
            item.SetValue("imageAlt", post.ImageAlt);
            item.SetValue("sourceUrl", post.SourceUrl);
            _contentService.Save(item);
            _contentService.Publish(item, Array.Empty<string>());
            created++;
        }

        _logger.LogInformation(
            "Champions 4 Heroes: blog section ready ({Created} new, {Backfilled} images added, {Total} total).",
            created,
            backfilled,
            Posts.Length);
    }

    /// <summary>
    /// Gives a post its seeded image when the live page is still missing one. Values an editor
    /// has set are never overwritten. Returns true when something changed.
    /// </summary>
    private bool AttachImage(IContent post, Post seed)
    {
        if (string.IsNullOrWhiteSpace(seed.Image))
        {
            return false;
        }

        // Compare against what is actually live, not the draft: a previous run may have
        // saved the value without ever publishing it.
        var published = _publishedCache.GetById(post.Key);

        bool liveMatches =
            published is not null
            && string.Equals(
                published.Value<string>("imageFile"),
                seed.Image,
                StringComparison.OrdinalIgnoreCase);

        if (liveMatches)
        {
            return false;
        }

        // Only fill blanks — an editor's own image is left alone. Set
        // C4H_FORCE_IMAGE_REFRESH=1 to re-assert the seeded filename after a rename.
        bool forceRefresh = Environment.GetEnvironmentVariable("C4H_FORCE_IMAGE_REFRESH") == "1";

        if (forceRefresh || string.IsNullOrWhiteSpace(post.GetValue<string>("imageFile")))
        {
            post.SetValue("imageFile", seed.Image);
        }

        if (string.IsNullOrWhiteSpace(post.GetValue<string>("imageAlt")))
        {
            post.SetValue("imageAlt", seed.ImageAlt);
        }

        _contentService.Save(post);
        _contentService.Publish(post, Array.Empty<string>());
        return true;
    }

    /// <summary>Loads the children of a node as full <see cref="IContent"/> items.</summary>
    private List<IContent> Children(int parentId) =>
        _entityService.GetChildren(parentId)
            .Select(child => _contentService.GetById(child.Id))
            .Where(content => content is not null)
            .Select(content => content!)
            .ToList();

    private IContentType CreateContentType(
        string alias,
        string name,
        (string Group, string GroupName, string Alias, string Name, int DataTypeId)[] fields,
        bool allowedAsRoot = false)
    {
        var contentType = new ContentType(_shortStringHelper, Constants.System.Root)
        {
            Alias = alias,
            Name = name,
            Icon = "icon-document",
            AllowedAsRoot = allowedAsRoot,
        };

        foreach (var field in fields)
        {
            var propertyType = new PropertyType(_shortStringHelper, GetDataType(field.DataTypeId))
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
            throw new InvalidOperationException($"Could not create the '{alias}' document type: {created.Result}");
        }

        IContentType saved = _contentTypeService.Get(alias) as IContentType ?? contentType;

        _contentTypeService
            .CreateTemplateAsync(
                saved.Key,
                alias == PostTypeAlias ? "Blog Post" : "Blog Listing",
                alias == PostTypeAlias ? "BlogPost" : "BlogListing",
                true,
                Constants.Security.SuperUserKey)
            .GetAwaiter()
            .GetResult();

        return _contentTypeService.Get(alias) as IContentType ?? saved;
    }

    /// <summary>Resolves a built-in data type (-88 textbox / -89 textarea) from the installed Umbraco data types.</summary>
    private IDataType GetDataType(int builtInId)
    {
        string editorAlias = builtInId == Constants.DataTypes.Textarea ? "Umbraco.Textarea" : "Umbraco.TextBox";

        IEnumerable<IDataType> matches =
            _dataTypeService.GetByEditorAliasAsync(editorAlias).GetAwaiter().GetResult();

        return matches.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"No data type using the '{editorAlias}' editor was found; the blog document types cannot be created.");
    }
}
