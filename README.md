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

```bash
dotnet run
```

Then open the URL printed in the console (the launch profile uses `http://localhost:50104`), or pin a port:

```bash
dotnet run --urls http://localhost:5099
```

- **Front office:** `/`
- **Back office:** `/umbraco`

### First run

On the very first start Umbraco will:

1. Create the SQLite database (`umbraco/Data/Umbraco.sqlite.db`) and install itself unattended.
2. Sign you in with the development administrator created from `appsettings.Development.json`:
   - email: `admin@champions4heroes.org`
   - password: `Champions4Heroes!`
3. Run `SiteSeedHandler`, which creates the **Home** document type (38 properties across 8 tabs), the
   `Home` template, and a published home page so `/` renders immediately.

> ⚠️ Change the administrator password and the `Unattended` settings in `appsettings.Development.json`
> before this goes anywhere public.

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

## Project layout

```
Program.cs                     Umbraco bootstrapping
Composers/SiteComposer.cs      Registers the seeding handler
Components/SiteSeedHandler.cs  Creates the document type + seeds the first page
Views/Home.cshtml              The page itself (all content read from Umbraco)
wwwroot/css/styles.css         Design system + responsive layout
wwwroot/js/main.js             Nav, scroll effects, contact form
appsettings*.json              Configuration (SQLite, unattended install)
umbraco/Data/*.sqlite.db       Local database (git-ignored — never committed)
```

## Resources featured on the site

| Resource | Link |
| --- | --- |
| *The Nathan Twining Story* (Fisher House Foundation) | https://www.youtube.com/watch?v=fmotPKW5c-s |
| Remembering Nate Twining (Friends of Fisher House – Illinois) | https://www.facebook.com/watch/?v=365086695465717 |
| Mel Twining on Facebook | https://www.facebook.com/nate.twining.733/ |

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

- Build and publish: `dotnet publish -c Release -o ./out`
- Host on any Windows/Linux host that runs .NET 10 (Azure App Service, Umbraco Cloud, IIS, Docker).
- Move the unattended install settings out of `appsettings.Development.json`, set a real
  connection string, and set `Umbraco:CMS:WebRouting:UmbracoApplicationUrl` so back-office emails work.

## Content notes

- Organization details, mission wording, and the Fisher House quote come from the resources above plus public
  sources (fisherhouse.org, iavmuseum.org, IRS 501(c)(3) listings).
