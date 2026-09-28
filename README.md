# Champions 4 Heroes

A single-page website for **Champions 4 Heroes**, a program of the **Michael Nathan Twining Foundation**
(EIN 85-2456580), founded by Mel Twining in honor of his son, Army Staff Sergeant Michael "Nate" Twining
of the 101st Airborne Division.

The site unites the organization's mission — advocating for, resourcing, and educating on traumatic brain
injury by bringing veterans and boxing champions together — with Nate's story.

## Resources featured on the site

| Resource | Link | Where it appears |
| --- | --- | --- |
| *The Nathan Twining Story* (Fisher House Foundation) | https://www.youtube.com/watch?v=fmotPKW5c-s | Hero poster, Story section, embedded in Watch |
| Remembering Nate Twining (Friends of Fisher House – Illinois) | https://www.facebook.com/watch/?v=365086695465717 | Watch section, footer |
| Mel Twining on Facebook | https://www.facebook.com/nate.twining.733/ | Contact section, footer |

## Files

```
index.html        All page content and structure
css/styles.css    Design system, layout, responsive rules
js/main.js        Nav, scroll effects, reveal animations, contact form
```

No build step and no dependencies — it's plain HTML, CSS, and JavaScript.

## Run it locally

Open `index.html` directly, or serve the folder:

```bash
# Python
python -m http.server 8080

# Node
npx serve .
```

Then visit http://localhost:8080

## Deploy

The folder is ready for any static host:

- **Netlify / Vercel / Cloudflare Pages** — drag-and-drop the folder or point at the repo, no build command.
- **GitHub Pages** — push and enable Pages on the root of the branch.

## Connecting the contact form

`js/main.js` has a `FORM_ENDPOINT` constant (currently `null`). Sign up for a form service such as
[Formspree](https://formspree.io) or [Basin](https://usebasin.com), then set:

```js
var FORM_ENDPOINT = "https://formspree.io/f/xxxxxxxx";
```

Until then the form validates input and shows an on-screen confirmation.

## Content notes

- Organization details, mission wording, and the Fisher House quote come from the resources above plus
  public sources (fisherhouse.org, iavmuseum.org, IRS 501(c)(3) listings).
- Replace or add photography as approved by the foundation — the current build uses the YouTube video
  stills, since no image assets were supplied.
