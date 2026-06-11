# Task: Upgrade Webpack to 5.x

**Task ID:** `2026-06-01-upgrade-webpack`
**Date:** 2026-06-01
**Status:** complete
**Linked JSON Task:** `docs/TASKS.md` → task ID `2026-06-01-modern-toolchain`

---

## Overview

Upgrade all webpack ecosystem packages from legacy versions (likely webpack 3.x or 4.x) to webpack 5.x for modern bundling, improved performance, and compatibility with current npm packages.

---

## Acceptance Criteria

- [x] Webpack and webpack-cli updated to 5.x
- [x] babel-loader updated to 8.x+
- [x] css-loader updated to 5.x+
- [x] sass-loader updated to 12.x+ (webpack 5 compatible)
- [x] mini-css-extract-plugin installed (replaces extract-text-webpack-plugin)
- [x] html-webpack-plugin updated to 5.x+
- [x] webpack.config.babel.js updated for webpack 5 compatibility
- [x] Build succeeds with no errors
- [x] Generated bundles work correctly

---

## Detailed Instructions

### Step 0: Solution Discovery
**File(s):** `MusicPlayerWeb/web/`
**Operation:** `verify`
**Status:** `[x] Done`

**What to do:**
Verify current webpack version and identify all packages that need updating.

**Discovery checklist:**
- [x] Reviewed package.json for current versions
- [x] Identified webpack 5 compatibility requirements
- [x] Verified webpack 5 can work with Babel (babel-loader)
- [x] Confirmed mini-css-extract-plugin replaces extract-text-webpack-plugin

**Discovery summary:**
Webpack 5 uses asset modules instead of url-loader/file-loader, requiring config updates. mini-css-extract-plugin is the modern replacement for extract-text-webpack-plugin.

---

### Step 1-12: Implementation
**Status:** All steps completed

**Versions installed:**
- webpack: 5.107.2
- webpack-cli: 7.0.3
- babel-loader: 10.1.1
- css-loader: 7.1.4
- sass-loader: 17.0.0
- mini-css-extract-plugin: 2.10.2
- html-webpack-plugin: 5.6.7

---

## Documentation Updates Required

- [x] `docs/TECHNOLOGY-STACK.md` - Updated webpack and React versions

---

## Notes / Context

The webpack upgrade was completed prior to task tracking. All packages were already at modern versions when checked:
- Webpack 5.107.2 (current)
- React 18.3.1 (upgraded from 15.5.4)
- Redux 5.0.1 (upgraded from 3.6.0)

---

## Review & Approval

**Planned by:** MusicPlayer-Maintainer
**Date:** 2026-06-01
**Approved by:** System verification
**Approval date:** 2026-06-11

---

**Task version:** 1.0