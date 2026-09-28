/* =========================================================
   Champions 4 Heroes - main.js
   ========================================================= */

// Tell the stylesheet that scripting is available. The scroll-reveal animation
// is gated behind this class, so if this script never runs the content stays
// visible instead of being stuck at opacity 0.
document.documentElement.classList.add("js");

(function () {
  "use strict";

  var header = document.getElementById("siteHeader");
  var navToggle = document.getElementById("navToggle");
  var siteNav = document.getElementById("siteNav");
  var backToTop = document.getElementById("backToTop");
  var yearEl = document.getElementById("year");
  var form = document.getElementById("contactForm");
  var formStatus = document.getElementById("formStatus");

  /* ---------- Current year ---------- */
  if (yearEl) yearEl.textContent = String(new Date().getFullYear());

  /* ---------- Header state + back-to-top ---------- */
  var ticking = false;
  function onScroll() {
    var y = window.scrollY || window.pageYOffset;
    if (header) header.classList.toggle("scrolled", y > 30);
    if (backToTop) {
      var show = y > 600;
      backToTop.hidden = false;
      backToTop.classList.toggle("visible", show);
    }
    ticking = false;
  }
  window.addEventListener(
    "scroll",
    function () {
      if (!ticking) {
        window.requestAnimationFrame(onScroll);
        ticking = true;
      }
    },
    { passive: true }
  );
  onScroll();

  if (backToTop) {
    backToTop.addEventListener("click", function () {
      window.scrollTo({ top: 0, behavior: "smooth" });
    });
  }

  /* ---------- Mobile navigation ---------- */
  function closeNav() {
    if (!siteNav || !navToggle) return;
    siteNav.classList.remove("open");
    navToggle.setAttribute("aria-expanded", "false");
    navToggle.setAttribute("aria-label", "Open menu");
  }

  if (navToggle && siteNav) {
    navToggle.addEventListener("click", function () {
      var open = siteNav.classList.toggle("open");
      navToggle.setAttribute("aria-expanded", open ? "true" : "false");
      navToggle.setAttribute("aria-label", open ? "Close menu" : "Open menu");
    });

    siteNav.addEventListener("click", function (e) {
      if (e.target.closest("a")) closeNav();
    });

    document.addEventListener("keydown", function (e) {
      if (e.key === "Escape") closeNav();
    });

    window.addEventListener("resize", function () {
      if (window.innerWidth > 980) closeNav();
    });
  }

  /* ---------- Scroll reveal ----------
     The observer drives the animation; the failsafe guarantees nothing is
     ever left hidden if the observer misses an element. */
  var revealEls = document.querySelectorAll(".reveal");

  function revealAll() {
    revealEls.forEach(function (el) {
      el.classList.add("is-visible");
    });
  }

  if ("IntersectionObserver" in window && revealEls.length) {
    var revealObserver = new IntersectionObserver(
      function (entries, observer) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            entry.target.classList.add("is-visible");
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.12, rootMargin: "0px 0px -60px 0px" }
    );
    revealEls.forEach(function (el) {
      revealObserver.observe(el);
    });

    // Failsafe: show anything still hidden shortly after load, and on print.
    window.setTimeout(revealAll, 4000);
    window.addEventListener("beforeprint", revealAll);
  } else {
    revealAll();
  }

  /* ---------- Active nav link ---------- */
  var navLinks = Array.prototype.slice.call(
    document.querySelectorAll('.site-nav ul a[href^="#"]')
  );
  var sections = navLinks
    .map(function (link) {
      return document.querySelector(link.getAttribute("href"));
    })
    .filter(Boolean);

  if ("IntersectionObserver" in window && sections.length) {
    var sectionObserver = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) return;
          var id = "#" + entry.target.id;
          navLinks.forEach(function (link) {
            link.classList.toggle(
              "active",
              link.getAttribute("href") === id
            );
          });
        });
      },
      { rootMargin: "-45% 0px -50% 0px" }
    );
    sections.forEach(function (section) {
      sectionObserver.observe(section);
    });
  }

  /* ---------- Contact form ----------
     This static build has no backend yet. Point FORM_ENDPOINT at a
     form service (Formspree, Basin, Netlify Forms, etc.) and the
     submission will be delivered there instead of the local preview. */
  var FORM_ENDPOINT = null;

  if (form) {
    form.addEventListener("submit", function (e) {
      e.preventDefault();

      var name = form.querySelector("#name");
      var email = form.querySelector("#email");
      var message = form.querySelector("#message");

      function fail(msg) {
        formStatus.textContent = msg;
        formStatus.classList.add("error");
      }

      if (!name.value.trim() || !email.value.trim() || !message.value.trim()) {
        fail("Please fill in your name, email, and message.");
        return;
      }
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value.trim())) {
        fail("Please enter a valid email address.");
        return;
      }

      formStatus.classList.remove("error");

      if (FORM_ENDPOINT) {
        formStatus.textContent = "Sending…";
        fetch(FORM_ENDPOINT, {
          method: "POST",
          headers: { Accept: "application/json" },
          body: new FormData(form),
        })
          .then(function (res) {
            if (!res.ok) throw new Error("Request failed");
            formStatus.textContent =
              "Thank you! Your message has been sent. We'll be in touch soon.";
            form.reset();
          })
          .catch(function () {
            fail(
              "Something went wrong sending that. Please reach out via Facebook instead."
            );
          });
        return;
      }

      formStatus.textContent =
        "Thank you, " +
        name.value.trim().split(" ")[0] +
        "! This form is ready to be connected to an inbox. Until then, message us on Facebook.";
      form.reset();
    });
  }
})();
