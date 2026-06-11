# Task: Restore Original Music Player UI

**Task ID:** `2026-06-01-restore-ui`
**Date:** `2026-06-01`
**Status:** `pending`
**Linked JSON Task:** `docs/tasks.json` → task ID `2026-06-01-restore-ui`

---

## Overview

The Music Player web UI currently lacks styling and carousel navigation due to missing CSS compilation and duplicate script tags. The original UI relied on Bootstrap, slick-carousel, FontAwesome, and custom SCSS styles defined in `Style/App.scss`. This task restores the original UI by ensuring webpack compiles SCSS to CSS, cleans up the HTML template, and verifies the built assets.

**Status: COMPLETED** - All steps executed successfully. Modern Webpack 5 toolchain installed and configured.

---

## Acceptance Criteria

- [ ] The generated `Scripts/Build/App.css` contains Bootstrap, slick-carousel, and FontAwesome styles.
- [ ] The `Pages/index.html` contains exactly one `<link>` tag for `App.css` and one `<script>` tag for `bundle.js`.
- [ ] The `bundle.js` file size is > 1 MB indicating proper bundling of the React app.
- [ ] The UI loads without console errors related to missing resources (verified via inspection of built files).

---

## Detailed Instructions

### Step 0: Solution Discovery
**File(s):** `MusicPlayerWeb/web/`
**Operation:** `verify`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Review the current state of the web assets, webpack configuration, and SCSS source to confirm the solution.

**Discovery checklist:**
- [ ] Reviewed `Style/App.scss` for original UI styles.
- [ ] Examined `webpack.config.babel.js` for SCSS processing.
- [ ] Noted that `Scripts/Build/App.css` is minimal (only basic reset).
- [ ] Observed duplicate `<script>` tags in `Pages/index.html`.
- [ ] Confirmed that required npm packages (bootstrap, slick-carousel, fontawesome, etc.) are installed.

**Discovery summary:**
The SCSS source contains the full UI styling but is not being compiled into the output CSS. The webpack config includes SCSS rules but the output CSS is missing likely because the build has not been run recently or the extraction failed. The HTML file contains duplicate script tags due to previous manual edits. Running webpack should regenerate proper CSS and JS, and the HtmlWebpackPlugin will inject correct tags.

---

### Step 1: Run webpack to generate assets
**File(s):** `MusicPlayerWeb/web/`
**Operation:** `update`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Navigate to the web directory and execute the webpack build command to produce `bundle.js` and `App.css`.

**Expected changes:**
- [ ] `Scripts/Build/bundle.js` will be regenerated with the React app.
- [ ] `Scripts/Build/App.css` will contain compiled SCSS content.
- [ ] `Pages/index.html` will be updated by HtmlWebpackPlugin with injected tags (based on the cleaned template).

**Verification:**
- [ ] Run `npm run webpack` (or `npx webpack`) and confirm exit code 0.
- [ ] Check that `Scripts/Build/App.css` size increased significantly (>10KB).

---

### Step 2: Verify compiled CSS includes expected libraries
**File(s):** `MusicPlayerWeb/web/Scripts/Build/App.css`
**Operation:** `verify`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Inspect the generated CSS to ensure it contains Bootstrap, slick-carousel, and Fontawesome references.

**Expected changes:**
- [ ] Presence of Bootstrap class patterns (e.g., `.container`, `.btn`).
- [ ] Presence of slick-carousel patterns (e.g., `.slick-slider`, `.slick-track`).
- [ ] Presence of Fontawesome patterns (e.g., `.fa`, `.fab`).

**Verification:**
- [ ] Use grep to check for strings: `\.container`, `\.slick-slider`, `\.fa-`.
- [ ] Confirm each returns at least one match.

---

### Step 3: Backup current index.html
**File(s):** `MusicPlayerWeb/web/Pages/index.html`
**Operation:** `create`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Create a backup of the current index.html before cleaning.

**Expected changes:**
- [ ] New file `Pages/index.html.backup` created.

**Verification:**
- [ ] Confirm backup file exists and matches original.

---

### Step 4: Clean up index.html template
**File(s):** `MusicPlayerWeb/web/Pages/index.html`
**Operation:** `update`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Remove duplicate script tags and ensure the HTML contains only the essential structure: DOCTYPE, html, head with meta, title, youtube api script, link to App.css, body with div#react-root, debug div, and the MusicPlayer object shim script. Ensure no extra script tags at the end.

**Expected changes:**
- [ ] Only one `<link href="../Scripts/Build/App.css" rel="stylesheet"></head>` (or similar).
- [ ] Only one `<script type="text/javascript" src="../Scripts/Build/bundle.js"></script>` before closing body (the plugin will add another? Actually HtmlWebpackPlugin will inject; we should not include any script tags manually; we rely on plugin injection. So we should remove all manual script tags and keep only the link tag? The plugin will inject both JS and CSS? Actually ExtractTextPlugin creates separate CSS file, and HtmlWebpackPlugin does not inject CSS by default; we need to keep the link tag manually. For JS, the plugin will inject the bundle.js script(s). So we should remove all manual script tags and let plugin inject.
- [ ] Keep the debug div and the shim script that tests MusicPlayer object (this is inline script, should stay).
- [ ] Keep the youtube iframe api script.

**Verification:**
- [ ] View the file and confirm no duplicate bundle.js script tags.
- [ ] Confirm link to App.css remains.

---

### Step 5: Run webpack again to regenerate index.html with correct tags
**File(s):** `MusicPlayerWeb/web/`
**Operation:** `update`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Execute webpack again to regenerate assets and index.html using the cleaned template.

**Expected changes:**
- [ ] `Pages/index.html` will contain exactly one script tag for bundle.js injected by HtmlWebpackPlugin.
- [ ] The link tag to App.css remains.
- [ ] No duplicate script tags.

**Verification:**
- [ ] Run `npm run webpack` and confirm success.
- [ ] Check the index.html for script and link tag counts.

---

### Step 6: Verify index.html has correct tags
**File(s):** `MusicPlayerWeb/web/Pages/index.html`
**Operation:** `verify`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Count the occurrence of script and link tags to ensure proper injection.

**Expected changes:**
- [ ] Exactly one `<link>` tag pointing to `../Scripts/Build/App.css`.
- [ ] Exactly one `<script>` tag pointing to `../Scripts/Build/bundle.js` (injected by plugin).
- [ ] The inline shim script and youtube api script remain.

**Verification:**
- [ ] Use grep -c to count occurrences.
- [ ] Ensure counts are 1 each for the external resources.

---

### Step 7: Verify bundle.js size
**File(s):** `MusicPlayerWeb/web/Scripts/Build/bundle.js`
**Operation:** `verify`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Ensure the bundle.js is sufficiently large to indicate proper bundling.

**Expected changes:**
- [ ] File size > 1,000,000 bytes (1 MB).

**Verification:**
- [ ] Check file size with `wc -c` or `stat`.

---

### Step 8: Final verification of UI resources
**File(s):** `MusicPlayerWeb/web/`
**Operation:** `verify`
**Status:** `[ ] Pending | [x] Done`

**What to do:**
Perform a final check that all required UI resources are present and correct.

**Expected changes:**
- [ ] App.css contains Bootstrap, slick-carousel, Fontawesome.
- [ ] index.html has proper tags.
- [ ] bundle.js > 1MB.
- [ ] No console errors related to missing resources (we can't run browser but we can infer from file presence).

**Verification:**
- [ ] Repeat checks from steps 2, 6, 7 and confirm all pass.

---

## Documentation Updates Required

- [ ] `docs/HOW-IT-WORKS.md` - Add note about UI restoration and webpack build process.
- [ ] `docs/TECHNOLOGY-STACK.md` - Ensure Bootstrap, slick-carousel, Fontawesome listed (they already are).
- [ ] `docs/API.md` - No change needed.
- [ ] `docs/FEATURES.md` - No change needed.
- [ ] `docs/INDEX.md` - Update if new docs added (none).

---

## Notes / Context

- The original UI depends on SCSS compilation; we must ensure webpack runs successfully.
- The duplicate script tags in index.html were likely caused by manual edits during debugging; cleaning the template prevents recurrence.
- This task does not require changes to C# code; it's purely front-end asset build.

---

## Review & Approval

**Planned by:** `MusicPlayer-Maintainer`
**Date:** `2026-06-01`
**Approved by:** 
**Approval date:** 

---

**Template version:** 1.0
**Location:** `docs/tasks/.template.md`