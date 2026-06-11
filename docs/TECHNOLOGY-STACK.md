# Technology Stack

Last updated: 2026-06-11
Status: Updated for .NET 10 + Avalonia + CefGlue.Avalonia (Cross-Platform)

---

## Overview

Music Player is a **cross-platform desktop application** upgraded to modern technologies:

- **Desktop Backend**: .NET 10 (`net10.0` for Windows/Linux/macOS)
- **UI Framework**: Avalonia 11.2.3 (cross-platform UI)
- **Browser Engine**: CefGlue.Avalonia 120.6099.211 (Chromium embedded, cross-platform, project reference from docs/CefGlue-main)
- **Web Frontend**: React with Redux
- **Audio Processing**: NAudio library (cross-platform with `WaveOutEvent`)
- **Build Optimization**: `web/node_modules/` excluded from output copy to prevent MSBuild file copy errors
- **Metadata**: TagLib# 2.1.0
- **YouTube**: YoutubeExplode library
- **Database**: SQLite via Microsoft.Data.Sqlite (EF Core 10)
- **Communication**: gRPC (replacing WCF duplex contracts)

---

## Complete Technology Listing

### 1. Core Platforms & Languages

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| .NET 10 | 10.0.104 SDK | Runtime | Cross-platform (net10.0, no Windows-specific targeting) |
| C# | 12.0 | Backend logic | Latest features, nullable enable |
| JavaScript | ES5/ES6 | Frontend | Babel transpiles modern syntax |
| JavaScript Object Model | N/A | Browser APIs | CefGlue.Avalonia exposes to JS via `AvaloniaCefBrowser` |

### 2. UI Frameworks

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| Avalonia | 11.2.3 | Desktop UI | Cross-platform (Windows/Linux/macOS) |
| Avalonia.Markup.Xaml | 11.2.3 | XAML processing | `.axaml` files instead of `.xaml` |
| CefGlue.Avalonia | 120.6099.211 | Chromium browser | Project reference from docs/CefGlue-main, cross-platform |
| React | 18.3.1 | Web UI | Modern version, hooks supported |
| React Router | 4.1.1 | Routing | Navigate between pages |
| React Redux | 9.3.0 | State management | Centralized state store |

### 3. Audio & Media Libraries

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| NAudio | 2.2.1 | Audio playback | Uses `WaveOutEvent` for cross-platform support (replaces `WaveOut` + `CoreAudioApi`) |
| TagLib# | 2.1.0 | Metadata | Read ID3 tags, Vorbis comments, etc. |
| YoutubeExplode | 6.3.10 | YouTube API | Parse video IDs, fetch playlists, channels |

### 4. Data & Persistence

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.0 | Database ORM | EF Core 10 with SQLite provider |
| Microsoft.EntityFrameworkCore.Tools | 10.0.0 | Migrations | Code-first migrations |
| Microsoft.Data.Sqlite | 10.0.0 | SQLite provider | Native SQLite support |

### 5. Communication Layer

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| gRPC | 2.67.0 | Inter-process communication | Replaces WCF duplex contracts |
| Grpc.AspNetCore | 2.67.0 | Server hosting | gRPC services in ASP.NET Core |
| Grpc.Net.Client | 2.67.0 | Client calls | Call gRPC services |
| Google.Protobuf | 3.29.3 | Message serialization | Protocol buffer messages |

### 6. YouTube Integration

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| YoutubeExplode | 6.3.10 | YouTube API | Parse video IDs, fetch playlists |
| YouTube Iframe API | N/A | Embed player | YouTube's official embed API |
| Dirble API Key | Embedded | Station data | Radio station discovery |

### 7. UI Components & Libraries

| Package | Purpose |
|------|------|
| @fortawesome/fontawesome-free | Icons |
| react-loader | Loading indicators |
| react-progress-circle | Circular progress display |
| react-waypoint | Scroll position tracking |
| react-slick | Carousel/slider components |

### 8. Development Tooling

| Tool | Purpose |
|------|------|
| dotnet CLI | Build orchestration (cross-platform) |
| NuGet | Package management |
| npm | Node.js dependencies |
| webpack | Bundling and module resolution (v5.107.2) |
| babel | JavaScript transpilation |
| babel-loader | Transpile JSX/JS modules |
| sass-loader | SCSS preprocessing |
| css-loader | CSS module support |
| html-webpack-plugin | Generate HTML entry points |
| mini-css-extract-plugin | Extract CSS to separate files (replaces extract-text-webpack-plugin) |

### 9. System Integration

| Technology | Purpose | Notes |
|------|------|------|
| Avalonia File Dialogs | Cross-platform file browsing | `OpenFolderDialog`, `OpenFileDialog` |
| .NET FileSystem | File access | `System.IO` for media scanning |
| PowerShell | Setup scripts (Windows-only) | `Initialize.ps1` for Windows setup |

### 10. Build Tools

| Tool | Purpose | Notes |
|------|------|------|
| dotnet CLI | Build orchestration | Supports `-r win-x64`, `-r linux-x64`, `-r osx-x64` |
| NuGet | Package management | Restores cross-platform packages |
| npm | Node.js dependencies | Frontend dependencies |
| webpack-cli | Webpack execution | Bundles React frontend |

### 11. External Dependencies

#### NAudio Dependencies
- Cross-platform audio APIs (WaveOutEvent)
- No Windows-specific CoreAudioApi
- Supports MP3, WAV, OGG, FLAC, etc.

#### TagLib# Dependencies
- Vorbis comments parsing
- ID3v1/ID3v2 standards
- FLAC metadata
- OGG metadata
- MP4 metadata
- AAC metadata
- Apple Lossless metadata

#### CefGlue.Avalonia Dependencies
- Chromium Embedded Framework (CEF 120)
- Platform-specific CEF binaries (`libcef.dll`, `libcef.so`, `libcef.dylib`)
- Xilium.CefGlue core library

#### YoutubeExplode Dependencies
- YouTube HTTP API
- Video parsing libraries
- Playlist data structures
- Channel uploads API

#### gRPC Dependencies
- HTTP/2 protocol
- Protocol Buffers
- Native gRPC C core library

---

## Architecture by Layer

### Presentation Layer (Desktop)
- Avalonia controls (MainWindow, `Avalonia.Controls`)
- `AvaloniaCefBrowser` (Chromium embedding)
- Cross-platform file dialogs
- FluentTheme (Avalonia default)

### Presentation Layer (Browser)
- React components
- HTML5 elements
- CSS/SASS styling
- YouTube Iframe
- CefGlue.Avalonia Chromium browser

### Business Logic Layer
- MusicPlayer namespace
  - Db.cs (data access, EF Core 10)
  - Factory.cs (factory + gRPC server/client)
  - Entity classes (Song, Album, etc.)
- MusicPlayerGate namespace
  - Actions.cs (commands)
  - Gateway (orchestration)
- MusicPlayerWeb (browser bridge, CefGlue.Avalonia)

### Communication Layer
- gRPC (MusicPlayerServerService, MusicPlayerClientService)
- HTTP (web requests)
- YouTube API HTTP
- Bidirectional streaming for WCF duplex replacement

### Data Layer
- SQLite database (EF Core 10)
- In-memory collections (for speed)
- FileSystem for media scanning

### File System Layer
- Local media folder scanning
- Direct file access via NAudio
- Stream reading without full load

---

## Technology Decisions Rationale

### Why .NET 10?
- Modern C# features (nullable, records, etc.)
- Cross-platform capability (net10.0, no Windows-specific targeting)
- Long-term support and updates
- Better performance than .NET Framework 4.5.2

### Why Avalonia instead of WPF?
- WPF is Windows-only
- Avalonia supports Windows/Linux/macOS
- XAML-like syntax (`.axaml`) with similar concepts
- Active community and regular updates

### Why CefGlue.Avalonia instead of CefSharp?
- CefSharp.Wpf is Windows-only
- CefSharp.Avalonia package does not exist
- CefGlue.Avalonia is the only Chromium + Avalonia cross-platform solution
- Requires API rewrite but provides full Chromium support

### Why gRPC?
- Replaces WCF duplex contracts
- Strong typing via Protocol Buffers
- Native support for bidirectional streaming
- Better .NET 10 integration than WCF
- Cross-platform compatibility

### Why EF Core 10?
- Replaces EF6 (incompatible with .NET 10)
- Native SQLite support via Microsoft.Data.Sqlite
- Better performance and LINQ support
- Code-first migrations

### Why React?
- Component reusability
- Virtual DOM efficiency
- Rich ecosystem
- Component-based architecture

### Why Redux?
- Unidirectional data flow
- Predictable state changes
- Middleware extensibility
- Simple debugging

### Why SQLite?
- Lightweight (embedded)
- No server installation
- ACID compliant
- EF Core support

---

## Package Sources

### NuGet Packages (Backend)
- Microsoft.EntityFrameworkCore.Sqlite 10.0.0
- Microsoft.EntityFrameworkCore.Tools 10.0.0
- Avalonia 11.2.3
- Avalonia.Markup.Xaml 11.2.3
- CefGlue.Avalonia 120.6099.1
- NAudio 2.2.1
- TagLib# 2.1.0
- YoutubeExplode 6.3.10
- Grpc.AspNetCore 2.67.0
- Grpc.Net.Client 2.67.0
- Google.Protobuf 3.29.3
- Newtonsoft.Json 13.0.3
- NLog 5.3.4
- AngleSharp 1.3.0

### npm Packages (Frontend)
- React ecosystem (via package.json)
- Babel ecosystem
- Webpack ecosystem
- FontAwesome icons

---

## Compatibility Matrix

| Platform | Backend | Frontend | Status |
|------|------|------|--------|
| Windows 10/11 (x64) | ✅ | ✅ | Full support, build tested |
| Linux (x64) | ✅ | ✅ | Full support, build tested |
| macOS (x64) | ✅ | ✅ | Full support, build tested (runtime pending) |

---

## License & Legal

### Open Source Components
- React: MIT
- Redux: MIT
- NAudio: BSD
- Avalonia: MIT
- CefGlue: MIT
- Webpack: MIT
- FontAwesome: CC BY 4.0 (free for personal)
- TagLib#: LGPL
- SQLite: Public Domain
- gRPC: Apache 2.0

### YouTube API
- TOS compliance required
- API key restrictions
- Rate limits apply

---

## Environment Setup

### .NET Environment
```bash
# Requirements
# .NET 10 SDK 10.0.104+
# Visual Studio 2022+ or VS Code with C# extension

# Restore packages
dotnet restore

# Build for current platform
dotnet build MusicPlayerWeb/MusicPlayerWeb.csproj

# Build for specific platforms
dotnet build -r win-x64 MusicPlayerWeb/MusicPlayerWeb.csproj
dotnet build -r linux-x64 MusicPlayerWeb/MusicPlayerWeb.csproj
dotnet build -r osx-x64 MusicPlayerWeb/MusicPlayerWeb.csproj
```

### Node Environment
```bash
# Node 8+ (React 15 era requirement)
# npm 5+
npm install
npm run webpack
```

---

## Migration History

### From .NET Framework 4.5.2 to .NET 10
1. **Framework**: .NET Framework 4.5.2 → .NET 10 (net10.0, cross-platform)
2. **UI**: WPF → Avalonia 11.2.3 (cross-platform)
3. **Browser**: CefSharp.Wpf → CefGlue.Avalonia 120.6099.1 (cross-platform Chromium)
4. **WCF → gRPC**: Duplex contracts replaced with gRPC services
5. **EF6 → EF Core 10**: Database ORM upgraded
6. **System.Data.SQLite → Microsoft.Data.Sqlite**: Provider changed
7. **Audio**: NAudio `WaveOut` + `CoreAudioApi` → `WaveOutEvent` (cross-platform)
8. **Packages**: All packages upgraded to .NET 10 compatible versions

### Breaking Changes
- WPF → Avalonia: `.xaml` → `.axaml`, namespace changes
- CefSharp → CefGlue.Avalonia: API changes (`LoadEnd` instead of `FrameLoadEnd`, no `MainFrame`)
- WCF duplex contracts replaced with gRPC bidirectional streaming
- EF6 APIs changed to EF Core 10
- YoutubeExplode 6.x API changes (VideoId, PlaylistVideo, IAsyncEnumerable)
- NAudio 1.8.4 → 2.2.1 API changes

---

## Known Issues

### CefGlue.Avalonia 120.6099.1
- **macOS runtime**: Not tested yet (build succeeds)
- **CEF binaries**: Must ship platform-specific binaries with application
- **API differences**: Requires full rewrite from CefSharp (no public parameterless constructor for `AvaloniaCefBrowser`)

### TagLib# 2.1.0
- **Warning**: NU1701 (restored using .NET Framework)
- **Status**: Works but shows compatibility warning

### YoutubeExplode 6.3.10
- **API Changes**: Major API changes from 4.x
- **IAsyncEnumerable**: Requires `await foreach` for video collections

### Volume Control
- Implemented via NAudio `WaveOutEvent.Volume`
- Cross-platform compatible (no Windows-specific APIs)

---

## Technology Summary

**Backend**: .NET 10 (net10.0) + Avalonia 11.2.3
**Frontend**: React 15.5.4 + Redux 5 + Webpack
**Audio**: NAudio 2.2.1 (WaveOutEvent) + TagLib# 2.1.0
**Browser**: CefGlue.Avalonia 120.6099.1 (Chromium)
**Database**: EF Core 10 + Microsoft.Data.Sqlite
**YouTube**: YoutubeExplode 6.3.10 + CefGlue.Avalonia
**Communication**: gRPC (replacing WCF duplex)
**Icons**: FontAwesome Free
**Styling**: SCSS → CSS (webpack)
**Build**: dotnet CLI (cross-platform) + Webpack + npm

**Supported Platforms**: Windows x64, Linux x64, macOS x64
**Total Size**: ~16 packages (backend), ~50 packages (frontend)
**Maintainer Effort**: Medium (modern stack, active support)
