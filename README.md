# Champions 4 Heroes

An **Umbraco 18** (.NET 10) website for **Champions 4 Heroes**, a program of the **Michael Nathan Twining
Foundation** (EIN 85-2456580) founded by Mel Twining in honor of his son, Army Staff Sergeant Michael "Nate"
Twining of the 101st Airborne Division.

The site unites the organization's mission — advocating for, resourcing, and educating on traumatic brain
injury by bringing veterans and boxing champions together — with Nate's story.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — `dotnet --version` should report 10.x
- Nothing else: Umbraco, SQLite, and the back office are all part of the project

## Run it

Copy the example settings and give yourself an admin login:

```bash
cp appsettings.Development.example.json appsettings.Development.json
```

Then edit `appsettings.Development.json` and set `UnattendedUserName`, `UnattendedUserEmail` and
`UnattendedUserPassword` to your own values. That file is git-ignored — credentials never get committed.

```bash
dotnet run
```

This uses the `Umbraco.Web.UI` launch profile, which listens on **both**:

| URL | Use |
| --- | --- |
| https://localhost:44369 | Front office **and back office** — the one you normally want |
| http://localhost:50104 | Front office only, over plain HTTP |

> ⚠️ **The back office must be opened over HTTPS.** Umbraco authenticates with OpenID Connect, and
> OpenIddict rejects plain HTTP with `This server only accepts HTTPS requests` (`ID2083`). If you
> restart with something like `dotnet run --urls http://localhost:5099` you lose the HTTPS endpoint and
> `/umbraco` will fail to log in. To move the ports, change `applicationUrl` in
> `Properties/launchSettings.json` and keep an `https://` entry.

If the browser warns about the certificate, trust the .NET dev certificate once:

```bash
dotnet dev-certs https --trust
```

- **Front office:** https://localhost:44369/
- **Back office:** https://localhost:44369/umbraco

### First run

On the very first start Umbraco will:

1. Create the SQLite database (`umbraco/Data/Umbraco.sqlite.db`) and install itself unattended.
2. Create the administrator from your `appsettings.Development.json`, so you can sign in at `/umbraco`.
3. Run `SiteSeedHandler`, which creates the **Home** document type (38 properties across 8 tabs), the
   `Home` template, and a published home page so `/` renders immediately.

> The unattended install only runs when the database is empty. To start over, stop the app and delete
> `umbraco/Data/*.sqlite.db*`.

Umbraco also needs to know its own public URL to build OAuth redirect URIs, so set it in
`appsettings.Development.json` to match the HTTPS port in `Properties/launchSettings.json`:

```json
"Umbraco": { "CMS": { "WebRouting": { "UmbracoApplicationUrl": "https://localhost:44369" } } }
```

## How the content is managed

Everything on the page comes from the **Home** content item — open the back office, edit, save, and publish:

| Back office tab | What it controls |
| --- | --- |
| Hero | Eyebrow, title, lead paragraph |
| Mission | Section title/lead and the three pillars (Advocacy, Resources, Education) |
| Nate's Story | Section title, two paragraphs, pull quote and attribution |
| What We Do | Section title/lead and the four programs |
| Watch & Share | Section title/lead, closing note, YouTube and Facebook video URLs |
| Get Involved | Section title/lead and the CTA banner heading |
| Contact & Footer | Contact intro, Facebook URL, mailing address, footer description |

`SiteSeedHandler` only creates the document type and content when they are **missing** — it never overwrites
edits made in the back office. To change the default wording for future installs, edit the `Fields` table in
`Components/SiteSeedHandler.cs`.

## Blog

`/blog` is a full Umbraco section with its own listing page and one page per post.

- **Listing page** (`blog` document type) at `/blog` — edit the title and lead paragraph there.
- **Posts** (`blogPost` document type) live under the listing. Each post has: Title, Posted by, Event,
  Body, Image file, Image alt text, and the original Facebook post URL.
- **Images** live in `wwwroot/img/blog/` and are referenced by filename in the `imageFile` field.
  Posts without a downloaded image fall back to a branded "4" placeholder.

`BlogSeedHandler` seeds 12 posts taken from the organization's Facebook posts, and only creates the
ones that are missing — so adding a post in the back office is never undone on restart. It also
attaches a seeded image to a post whose **live** page is missing one, but never overwrites an image
or alt text an editor has set. To change the seeded posts, edit the `Posts` table in
`Components/BlogSeedHandler.cs`.

Every post has a filename reserved for it, so dropping a file into `wwwroot/img/blog/` is all it
takes. If you rename one in the seed, start the app once with `C4H_FORCE_IMAGE_REFRESH=1` to re-assert
the new name, then start normally again.

| Post | Image file |
| --- | --- |
| Champions 4 Heroes at the WBC and National Boxing Hall of Fame | `wbc-national-boxing-hall-of-fame.jpg` |
| Road to Canastota: IBHOF Weekend | `trilogy.jpg` |
| On Our Way to the Atlantic City Boxing Hall of Fame | `usteam.jpg` |
| Macho Time: Hall of Fame Weekend | `macho-time-hall-of-fame-weekend.jpg` |
| Thank You, Janie | `janie-bracero-thank-you.jpg` |
| Champions 4 Heroes at the Indiana Sports Hall of Fame | `indiana-sports-hall-of-fame.jpg` |
| NBHOF Photo Drop: Late Uploads | `mosley.jpg` |
| Congratulations to Brenda Spinks | `brenda-spinks-award.jpg` |
| The Spinks Family and Champions 4 Heroes | `bernardhopkins.jpg` |
| What Champions 4 Heroes Stands For | `completeteam.jpg` |
| Champions Gift to the VA at Hines | `hines-va-champions-gift.jpg` |
| Hero's Welcome in Bacolod | `bacolod-hero-welcome-nietes.jpg` |

All twelve posts have an image and alt text. Facebook's crawler endpoint served six of the images;
the rest were login-walled and dropped in by hand. If a post points at a file that is not on disk, the
card falls back to the branded "4" placeholder instead of a broken image.

Most of the photos are portrait phone shots, so images are cropped from the top (`object-position:
center top`) to keep faces in frame.

Alt text is intentionally left empty for every image. Add a description in the back office
(**Image alt text**) when you have one written — an empty alt is invisible to screen readers, so this
is worth doing before launch. An editor's value is never overwritten once set.

## Project layout

```
Program.cs                     Umbraco bootstrapping
Composers/SiteComposer.cs      Registers the seeding handlers
Components/SiteSeedHandler.cs  Creates the home document type + seeds the first page
Components/BlogSeedHandler.cs  Creates the blog types + seeds the posts
Views/Home.cshtml              The home page (all content read from Umbraco)
Views/BlogListing.cshtml       /blog — the post grid
Views/BlogPost.cshtml          A single blog post
wwwroot/img/blog/              Blog post images
wwwroot/css/styles.css         Design system + responsive layout
wwwroot/js/main.js             Nav, scroll effects, contact form
appsettings.json               Shared configuration
appsettings.Development.json   Local dev settings — git-ignored, holds your admin login
appsettings.Local.json         Optional per-machine override, also git-ignored
umbraco/Data/*.sqlite.db       Local database (git-ignored — never committed)
```

## Resources featured on the site

| Resource | Link |
| --- | --- |
| *The Nathan Twining Story* (Fisher House Foundation) | https://www.youtube.com/watch?v=fmotPKW5c-s |
| Remembering Nate Twining (Friends of Fisher House – Illinois) | https://www.facebook.com/watch/?v=365086695465717 |
| Mel Twining on Facebook | https://www.facebook.com/nate.twining.733/ |

The 12 blog posts were sourced from the organization's Facebook posts (shared via Eva D. Jones-Young,
Janie Bracero, Brenda Glur Spinks, Hector Camacho Jr., Bacolod City PIO, Hines VA Hospital, and
Ritchel N Twining). Each post links back to its original Facebook URL.

Copy and imagery: the site uses the YouTube video stills, since no photo assets were supplied. Drop real
photography into `wwwroot/media` and pick it up as Umbraco media when available.

## Connecting the contact form

`wwwroot/js/main.js` has a `FORM_ENDPOINT` constant (currently `null`). Point it at a form service such as
[Formspree](https://formspree.io) or [Basin](https://usebasin.com) and submissions will be delivered there:

```js
var FORM_ENDPOINT = "https://formspree.io/f/xxxxxxxx";
```

Until then the form validates input and shows an on-screen confirmation.

## Deploying

The live site is a **static export**. Netlify, GitHub Pages and Cloudflare Pages host static files;
they cannot run Umbraco itself, because the app needs a persistent filesystem for its SQLite
database and a long-running process for background jobs. So Umbraco is the editing tool, and the
export is what gets published.

### Exporting locally

```bash
dotnet run                                    # in one terminal
node tools/export-static.js http://localhost:50104
```

That writes `dist/`: 14 prerendered pages (`/`, `/blog/`, and the 12 posts) plus the styles,
scripts and images. Any new post is picked up automatically, because the export follows the site's
own links rather than a hardcoded list.

### GitHub Pages

`.github/workflows/deploy-pages.yml` does the whole thing on every push to `main`: it builds the
app, starts it so it seeds itself, exports the static site, and publishes `dist/` to Pages.

To switch it on, in the repo go to **Settings → Pages → Build and deployment → Source → GitHub
Actions**. The site then appears at `https://hinlocaesar.github.io/champion-4-heroes/`.

The workflow passes the base path `/champion-4-heroes` to the export, which rewrites root-relative
URLs so the site works from a sub-path. If you later attach a custom domain and serve from the
domain root, change that one argument to `""`.

### Netlify or Cloudflare Pages

Either host works from the same `dist/` folder — no build command needed, since the workflow
already produces it. Copy the folder contents into a new site and set the publish directory to
`dist` (or `dist` with no build command on Netlify).

### What deploying statically means for editing

The back office at `/umbraco` is a **local tool only** — it is not deployed. Content changes flow
like this:

1. Edit and publish in the local back office (or edit the seed tables in `Components/`)
2. Re-run the export
3. Commit and push, which re-runs the workflow

Because the content lives in version-controlled code, every content change is reviewable in a pull
request and the deployed site is always reproducible from the repository.


## Content notes

- Organization details, mission wording, and the Fisher House quote come from the resources above plus public
  sources (fisherhouse.org, iavmuseum.org, IRS 501(c)(3) listings).
