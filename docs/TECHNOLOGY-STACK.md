# Technology Stack

## Overview

Music Player has been upgraded to modern technologies:

- **Desktop Backend**: .NET 10 WPF application
- **Browser Engine**: CefSharp (Chromium embedded) - Windows only
- **Web Frontend**: React with Redux
- **Audio Processing**: NAudio library
- **Metadata**: TagLib#
- **YouTube**: YoutubeExplode library
- **Database**: SQLite via Microsoft.Data.Sqlite (EF Core 10)
- **Communication**: gRPC (replacing WCF)

## Complete Technology Listing

### 1. Core Platforms & Languages

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| .NET 10 | 10.0.104 SDK | Runtime | Cross-platform, WPF requires net10.0-windows |
| C# | 12.0 | Backend logic | Latest features, nullable enable |
| JavaScript | ES5/ES6 | Frontend | Babel transpiles modern syntax |
| JavaScript Object Model | N/A | Browser APIs | CefSharp exposes to JS |

### 2. UI Frameworks

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| WPF (Windows Presentation Foundation) | net10.0-windows | Desktop UI | MainWindow, file dialogs |
| React | 15.5.4 | Web UI | Legacy version, consider upgrading |
| React Router | 4.1.1 | Routing | Navigate between pages |
| React Redux | 5.0.5 | State management | Centralized state store |
| CefSharp | 128.4.90 | Chromium browser | Embeds Chrome in .NET, Windows only |

### 3. Audio & Media Libraries

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| NAudio | 2.2.1 | Audio playback | Core audio I/O, supports multiple formats |
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
| webpack | Bundling and module resolution |
| babel | JavaScript transpilation |
| babel-loader | Transpile JSX/JS modules |
| sass-loader | SCSS preprocessing |
| css-loader | CSS module support |
| html-webpack-plugin | Generate HTML entry points |
| extract-text-webpack-plugin | Split CSS from JS bundles |
| node-sass | SCSS compiler |

### 9. System Integration

| Technology | Purpose |
|------|------|
| Windows COM/FileSystem | File browsing |
| Windows Registry | Installer configuration |
| PowerShell | Setup scripts (Initialize.ps1) |

### 10. Build Tools

| Tool | Purpose |
|------|------|------|
| dotnet CLI | Build orchestration |
| NuGet | Package management |
| npm | Node.js dependencies |
| webpack-cli | Webpack execution |
| ps1 | PowerShell scripts |

### 11. External Dependencies

#### NAudio Dependencies
- Windows Audio API
- DirectShow filter graph
- WASAPI interface
- AudioFormat information

#### TagLib# Dependencies
- Vorbis comments parsing
- ID3v1/ID3v2 standards
- FLAC metadata
- OGG metadata
- MP4 metadata
- AAC metadata
- Apple Lossless metadata

#### CefSharp Dependencies
- Chromium Embedded Framework
- Windows CRT DLLs
- VCRUNTIME library
- Visual C++ Redistributables
- **Note**: Version 128.4.90 has known high severity vulnerability (NU1903)

#### YoutubeExplode Dependencies
- YouTube HTTP API
- Video parsing libraries
- Playlist data structures
- Channel uploads API

#### gRPC Dependencies
- HTTP/2 protocol
- Protocol Buffers
- Native gRPC C core library

## Architecture by Layer

### Presentation Layer (Desktop)
- WPF controls (MainWindow, System.Windows.Media)
- DataTemplate for list views
- ICommand for MVVM patterns
- System.Windows.Shell for app bar

### Presentation Layer (Browser)
- React components
- HTML5 elements
- CSS/SASS styling
- YouTube Iframe
- CefSharp Chromium browser

### Business Logic Layer
- MusicPlayer namespace
  - Db.cs (data access, EF Core 10)
  - Factory.cs (factory + gRPC server/client)
  - Entity classes (Song, Album, etc.)
- MusicPlayerGate namespace
  - Actions.cs (commands)
  - Gateway (orchestration)
- MusicPlayerWeb (browser bridge, CefSharp)

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

## Technology Decisions Rationale

### Why .NET 10?
- Modern C# features (nullable, records, etc.)
- Cross-platform capability (with EnableWindowsTargeting for WPF)
- Long-term support and updates
- Better performance than .NET Framework 4.5.2

### Why gRPC?
- Replaces WCF duplex contracts
- Strong typing via Protocol Buffers
- Native support for bidirectional streaming
- Better .NET 10 integration than WCF
- Cross-platform compatibility

### Why CefSharp?
- Modern rendering (Chromium)
- YouTube embed required
- Custom protocols possible
- Cross-browser compatibility
- **Limitation**: Windows-only, no net10.0-windows support on Linux

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

## Package Sources

### NuGet Packages (Backend)
- Microsoft.EntityFrameworkCore.Sqlite 10.0.0
- Microsoft.EntityFrameworkCore.Tools 10.0.0
- NAudio 2.2.1
- TagLib# 2.1.0
- YoutubeExplode 6.3.10
- Grpc.AspNetCore 2.67.0
- Grpc.Net.Client 2.67.0
- Google.Protobuf 3.29.3
- Newtonsoft.Json 13.0.3
- NLog 5.3.4
- AngleSharp 1.3.0
- CefSharp.Wpf 128.4.90 (Windows only)

### npm Packages (Frontend)
- React ecosystem (via package.json)
- Babel ecosystem
- Webpack ecosystem
- FontAwesome icons

## Compatibility Matrix

| Platform | Backend | Frontend | Notes |
|------|------|------|--------|
| Windows 10/11 | ✅ | ✅ | Primary platform, full support |
| Windows 7+ | ✅ | ✅ | Legacy Windows support |
| Linux | ✅ | ⚠️ | Backend works, CefSharp blocked (Windows-only) |
| macOS | ✅ | ⚠️ | Backend works, CefSharp blocked (Windows-only) |

## License & Legal

### Open Source Components
- React: MIT
- Redux: MIT
- NAudio: BSD
- CefSharp: MIT
- Webpack: MIT
- FontAwesome: CC BY 4.0 (free for personal)
- TagLib#: LGPL
- SQLite: Public Domain
- gRPC: Apache 2.0

### YouTube API
- TOS compliance required
- API key restrictions
- Rate limits apply

## Environment Setup

### .NET Environment
```bash
# Requirements
# .NET 10 SDK 10.0.104+
# Visual Studio 2022+ or VS Code with C# extension

# Restore packages
dotnet restore

# Build (Windows)
dotnet build MusicPlayerWeb.sln

# Build (Linux/macOS - MusicPlayer only)
dotnet build MusicPlayer/MusicPlayer.csproj
```

### Node Environment
```bash
# Node 8+ (React 15 era requirement)
# npm 5+
npm install
npm run webpack
```

## Migration History

### From .NET Framework 4.5.2 to .NET 10
1. **Framework**: .NET Framework 4.5.2 → .NET 10
2. **WCF → gRPC**: Duplex contracts replaced with gRPC services
3. **EF6 → EF Core 10**: Database ORM upgraded
4. **System.Data.SQLite → Microsoft.Data.Sqlite**: Provider changed
5. **Packages**: All packages upgraded to .NET 10 compatible versions

### Breaking Changes
- WCF duplex contracts replaced with gRPC bidirectional streaming
- EF6 APIs changed to EF Core 10
- YoutubeExplode 6.x API changes (VideoId, PlaylistVideo, IAsyncEnumerable)
- CefSharp 63 → 128 API changes (DisplayHandler, SchemeHandlerFactory)
- NAudio 1.8.4 → 2.2.1 API changes

## Known Issues

### CefSharp.Wpf 128.4.90
- **Vulnerability**: NU1903 (high severity)
- **Platform**: No net10.0-windows support on Linux
- **Status**: Blocked on Linux, works on Windows

### TagLib# 2.1.0
- **Warning**: NU1701 (restored using .NET Framework)
- **Status**: Works but shows compatibility warning

### YoutubeExplode 6.3.10
- **API Changes**: Major API changes from 4.x
- **IAsyncEnumerable**: Requires `await foreach` for video collections

## Technology Summary

**Backend**: .NET 10 + WPF (net10.0-windows)
**Frontend**: React 15.5.4 + Redux 5 + Webpack
**Audio**: NAudio 2.2.1 + TagLib# 2.1.0
**Database**: EF Core 10 + Microsoft.Data.Sqlite
**YouTube**: YoutubeExplode 6.3.10 + CefSharp 128.4.90
**Communication**: gRPC (replacing WCF duplex)
**Icons**: FontAwesome Free
**Styling**: SCSS → CSS (webpack)
**Build**: dotnet CLI + Webpack + npm

**Total Size**: ~15 packages (backend), ~50 packages (frontend)
**Maintainer Effort**: Medium (modern stack, active support)
