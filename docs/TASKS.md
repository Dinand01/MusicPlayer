# Task Registry

**Meta:**
- **Agent Name:** MusicPlayer-Maintainer
- **Storage File:** `docs/TASKS.md` (PRIMARY - edit this file)
- **Last Updated:** 2026-06-11
- **Version:** 2.3

> **Note:** This markdown file is the primary task registry. Edit this file to add/modify tasks.

---

## Rules

- **Documentation Required:** ✅ true
- **Plan Every New Task:** ✅ true
- **Atomic Steps:** ✅ true
- **Verify Before Complete:** ✅ true
- **Auto Create Plan:** ❌ false
- **Task File Required:** ✅ true

---

## Documentation Files Mapping

| Purpose | File | Triggers |
|---------|------|----------|
| Architecture | `docs/architecture.md` | component added/removed, architecture changed, file structure modified, code flow changed |
| Features | `docs/features.md` | new feature added, feature modified, deprecated feature removed |
| Technology Stack | `docs/technology-stack.md` | library added, library updated, dependency removed, new technology adopted |
| How It Works | `docs/how-it-works.md` | new workflow documented, inner working changed, algorithm modified, flow diagram updated |
| API | `docs/api.md` | API endpoint added, function signature changed, state model modified, protocol changed |
| Index | `docs/index.md` | any documentation change, TOC needs updating, link broken or changed |

---

## Active Tasks (3)

### Task 3: Upgrade Webpack to 5.x

**ID:** `2026-06-01-modern-toolchain`  
**Status:** complete  
**Task File:** `docs/tasks/2026-06-01-upgrade-webpack.md`

#### Steps:

| # | Action | File(s) | Status |
|---|--------|---------|--------|
| 0 | Solution Discovery | `MusicPlayerWeb/web/` | ✅ |
| 1 | Backup current package.json and webpack config | `MusicPlayerWeb/web/package.json` | ✅ |
| 2 | Update webpack and webpack-cli to version 5.x | `MusicPlayerWeb/web/package.json` | ✅ |
| 3 | Update babel-loader to version 8.x+ | `MusicPlayerWeb/web/package.json` | ✅ |
| 4 | Update css-loader to version 5.x+ | `MusicPlayerWeb/web/package.json` | ✅ |
| 5 | Update sass-loader to version 12.x+ | `MusicPlayerWeb/web/package.json` | ✅ |
| 6 | Update extract-text-webpack-plugin or replace with mini-css-extract-plugin | `MusicPlayerWeb/web/package.json` | ✅ |
| 7 | Update html-webpack-plugin to version 5.x+ | `MusicPlayerWeb/web/package.json` | ✅ |
| 8 | Update url-loader and file-loader to versions compatible with webpack 5 | `MusicPlayerWeb/web/package.json` | ✅ |
| 9 | Update webpack.config.babel.js for webpack 5 compatibility | `MusicPlayerWeb/web/webpack.config.babel.js` | ✅ |
| 10 | Test webpack build with updated dependencies | `MusicPlayerWeb/web/` | ✅ |
| 11 | Verify generated assets | `MusicPlayerWeb/web/Scripts/Build/` | ✅ |
| 12 | Update documentation | `docs/TECHNOLOGY-STACK.md` | ✅ |

---

### Task 2: Restore Original Music Player UI

**ID:** `2026-06-01-restore-ui`  
**Status:** complete  
**Task File:** `docs/tasks/2026-06-01-restore-ui.md`

#### Steps:

| # | Action | File(s) | Status |
|---|--------|---------|--------|
| 0 | Solution Discovery | `MusicPlayerWeb/web/` | ✅ |
| 1 | Install missing dev dependencies | `MusicPlayerWeb/web/` | ✅ |
| 2 | Fix SCSS import and HTML CDN link | `MusicPlayerWeb/web/Style/App.scss` | ✅ |
| 3 | Run webpack to generate assets | `MusicPlayerWeb/web/` | ✅ |
| 4 | Verify compiled CSS includes expected libraries | `MusicPlayerWeb/web/Scripts/Build/App.css` | ✅ |
| 5 | Backup current index.html | `MusicPlayerWeb/web/Pages/index.html` | ✅ |
| 6 | Clean up index.html template | `MusicPlayerWeb/web/Pages/index.html` | ✅ |
| 7 | Run webpack again to regenerate index.html with correct tags | `MusicPlayerWeb/web/` | ✅ |
| 8 | Verify index.html has correct tags | `MusicPlayerWeb/web/Pages/index.html` | ✅ |
| 9 | Verify bundle.js size | `MusicPlayerWeb/web/Scripts/Build/bundle.js` | ✅ |
| 10 | Final verification of UI resources | `MusicPlayerWeb/web/` | ✅ |

### Task 1: Upgrade to .NET 10

**ID:** `2026-05-06-upgrade-dotnet-10`  
**Status:** complete  
**Task File:** `docs/tasks/2026-05-06-upgrade-dotnet-10.md`

#### Steps:

| # | Action | File(s) | Status |
|---|--------|---------|--------|
| 0 | Solution Discovery - Research WCF contracts, gRPC/Websockets, component compatibility | `MusicPlayer/Controller/WCFServerClient.cs, MusicPlayer/Controller/WCFServerService.cs` | ✅ |
| 1 | Convert MusicPlayer.csproj to SDK-style format | `MusicPlayer/MusicPlayer.csproj` | ✅ |
| 2 | Convert MusicPlayerWeb.csproj (WPF) to SDK-style format | `MusicPlayerWeb/MusicPlayerWeb.csproj` | ✅ |
| 3 | Update target framework to net10.0 in both projects | `MusicPlayer/MusicPlayer.csproj, MusicPlayerWeb/MusicPlayerWeb.csproj` | ✅ |
| 4 | Research gRPC compatibility with current WCF contract | `MusicPlayer/Interface/IClientContract.cs, MusicPlayer/Interface/IServerContract.cs` | ✅ |
| 5 | Implement gRPC services (preferred) OR Websocket transport layer | `MusicPlayer/Protos/ or MusicPlayer/Controllers/WebSocket/` | ✅ |
| 6 | Refactor WCFServerClient.cs → gRPC client / Websocket client | `MusicPlayer/Controller/WCFServerClient.cs` | ✅ |
| 7 | Refactor WCFServerService.cs → gRPC server / Websocket server | `MusicPlayer/Controller/WCFServerService.cs` | ✅ |
| 8 | Update interfaces for new transport | `MusicPlayer/Interface/IClientContract.cs, MusicPlayer/Interface/IServerContract.cs` | ✅ |
| 9 | Upgrade CefSharp.Wpf 63 → latest CefSharp.Wpf | `MusicPlayerWeb/MusicPlayerWeb.csproj` | ✅ |
| 10 | Migrate EntityFramework 6 → EF Core 10 | `MusicPlayer/Db.cs, MusicPlayer/Models/` | ✅ |
| 11 | Replace System.Data.SQLite → Microsoft.Data.Sqlite | `MusicPlayer/MusicPlayer.csproj, MusicPlayer/Db.cs` | ✅ |
| 12 | Update NAudio to .NET 10 compatible version | `MusicPlayer/MusicPlayer.csproj` | ✅ |
| 13 | Update all remaining NuGet packages | `MusicPlayer/MusicPlayer.csproj, MusicPlayerWeb/MusicPlayerWeb.csproj` | ✅ |
| 14 | Fix code breaking changes for .NET 10 | `MusicPlayer/, MusicPlayerWeb/` | ✅ |
| 15 | Update MusicPlayerWrapper.cs (WPF-specific code) | `MusicPlayer/Controller/MusicPlayerWrapper.cs` | ✅ |
| 16 | Test WPF UI loads correctly with CefSharp | `MusicPlayerWeb/` | ✅ |
| 17 | Migrate to CefGlue.Avalonia (replacing CefSharp.Wpf) | `MusicPlayerWeb/MainWindow.xaml.cs, MusicPlayerWeb/CefComponents/DisplayHandler.cs, MusicPlayerWeb/SchemeHandlerFactory.cs` | ✅ |
| 18 | Re-implement gRPC server/client (restore from .bak) | `MusicPlayer/Controller/GrpcServerService.cs, MusicPlayer/Controller/GrpcClientContract.cs, MusicPlayer/Protos/musicplayer.proto` | ✅ |
| 19 | Fix YoutubeExplode 6.x API integration | `MusicPlayer/Models/VideoInfo.cs, MusicPlayer/Interface/IVideo.cs, MusicPlayer/Controller/VideoController.cs` | ✅ |
| 20 | Implement gRPC server hosting in Factory.cs | `MusicPlayer/Factory.cs, MusicPlayer/Controller/ServerHost.cs.bak` | ✅ |
| 21 | Implement gRPC client connection in Factory.cs | `MusicPlayer/Factory.cs, MusicPlayer/Controller/ClientConnection.cs.bak` | ✅ |
| 22 | Restore Javascript interop in MusicPlayerGate with CefGlue.Avalonia | `MusicPlayerWeb/MusicPlayerGate.cs, MusicPlayerWeb/MusicPlayerGate.Data.cs` | ✅ |
| 23 | Fix remaining TODOs (MusicPlayer.cs thread exception) | `MusicPlayer/Controller/MusicPlayer.cs` | ✅ |
| 24 | Final integration test - full application build and test | `MusicPlayer.sln` | ✅ |
| 25 | Solution Discovery - Cross-Platform Audit | `docs/tasks/2026-05-06-upgrade-dotnet-10.md` | ✅ |
| 26 | Update TECHNOLOGY-STACK.md | `docs/TECHNOLOGY-STACK.md` | ✅ |
| 27 | Update ARCHITECTURE.md | `docs/ARCHITECTURE.md` | ✅ |
| 28 | Update HOW-IT-WORKS.md | `docs/HOW-IT-WORKS.md` | ✅ |
| 29 | Update API.md | `docs/API.md` | ✅ |
| 30 | Update INDEX.md | `docs/INDEX.md` | ✅ |
| 31 | Cross-platform audio testing (including volume control) | `MusicPlayer/Controller/MusicPlayer.cs` | ✅ |

---
