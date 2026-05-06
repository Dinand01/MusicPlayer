# Technology Stack

## Overview

Music Player uses a hybrid architecture combining:
- **Desktop Backend**: .NET WPF application
- **Browser Engine**: CefSharp (Chromium embedded)
- **Web Frontend**: React with Redux
- **Audio Processing**: NAudio library
- **Metadata**: TagLib#
- **YouTube**: YoutubeExplode library
- **Database**: SQLite via System.Data.SQLite

## Complete Technology Listing

### 1. Core Platforms & Languages

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| .NET Framework | 4.5.2 | Runtime | Legacy .NET, Windows-only |
| C# | Latest compatible | Backend logic | Type-safe, cross-compilable |
| JavaScript | ES5/ES6 | Frontend | Babel transpiles modern syntax |
| JavaScript Object Model | N/A | Browser APIs | CefSharp exposes to JS |

### 2. UI Frameworks

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| WPF (Windows Presentation Foundation) | 4.5.2 | Desktop UI | MainWindow, file dialogs |
| React | 15.5.4 | Web UI | Legacy version, consider upgrading |
| React Router | 4.1.1 | Routing | Navigate between pages |
| React Redux | 5.0.5 | State management | Centralized state store |
| CefSharp | Latest | Chromium browser | Embeds Chrome in .NET |

### 3. Audio & Media Libraries

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| NAudio | Latest | Audio playback | Core audio I/O |
| TagLib# | 2.1.0 | Metadata | Read ID3 tags, Vorbis comments, etc. |
| CefSharp | Latest | YouTube playback | Renders YouTube videos |

### 4. Data & Persistence

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| System.Data.SQLite | 1.0.108 | Database | Embedded SQLite for metadata |
| SQLite Linq | 1.0.108 | LINQ support | Query SQLite with LINQ |
| System.Data.SQLite.Core | 1.0.108 | Core SQLite | Native binding |

### 5. YouTube Integration

| Technology | Version | Purpose | Notes |
|------|------|------|--------|
| YoutubeExplode | 4.2.4 | YouTube API | Parse video IDs, fetch playlists |
| YouTube Iframe API | N/A | Embed player | YouTube's official embed API |
| Dirble API Key | Embedded | Station data | Radio station discovery |

### 6. UI Components & Libraries

| Package | Purpose |
|------|------|
| @fortawesome/fontawesome-free | Icons |
| react-loader | Loading indicators |
| react-progress-circle | Circular progress display |
| react-waypoint | Scroll position tracking |
| react-slick | Carousel/slider components |

### 7. Development Tooling

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

### 8. System Integration

| Technology | Purpose |
|------|------|
| Windows COM/FileSystem | File browsing |
| Windows Registry | Installer configuration |
| PowerShell | Setup scripts (Initialize.ps1) |

### 9. Build Tools

| Tool | Purpose |
|------|------|------|------|
| msbuild | Build orchestration |
| nuget | Package management |
| npm | Node.js dependencies |
| webpack-cli | Webpack execution |
| ps1 | PowerShell scripts |

### 10. External Dependencies

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

#### YoutubeExplode Dependencies
- YouTube HTTP API
- Video parsing libraries
- Playlist data structures

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

### Business Logic Layer
- MusicPlayer namespace
  - Db.cs (data access)
  - Factory.cs (singleton)
  - Entity classes (Song, Album, etc.)
- MusicPlayerGate namespace
  - Actions.cs (commands)
  - Gateway (orchestration)
- MusicPlayerWeb (browser bridge)

### Data Layer
- SQLite database
- In-memory collections (for speed)
- FileSystem for media scanning

### Communication Layer
- TCP Socket (streaming protocol)
- HTTP (web requests)
- YouTube API HTTP

### File System Layer
- Local media folder scanning
- Direct file access via NAudio
- Stream reading without full load

## Technology Decisions Rationale

### Why .NET Framework?
- Windows platform alignment
- Rich UI capabilities (WPF)
- Extensive audio libraries (NAudio)
- Maturity and stability
- File system integration

### Why CefSharp?
- Modern rendering (Chromium)
- YouTube embed required
- Custom protocols possible
- Cross-browser compatibility

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
- LINQ support

### Why TCP Streaming?
- Direct audio transfer
- Minimal overhead
- Synchronization control
- Cross-client broadcasting

## Package Sources

### NuGet Packages (Backend)
- System.Data.SQLite.*
- taglib-sharp
- YoutubeExplode
- NAudio (via nuget)
- CefSharp

### npm Packages (Frontend)
- React ecosystem (via package.json)
- Babel ecosystem
- Webpack ecosystem
- FontAwesome icons

## Technology Roadmap

### Current State
- Legacy technology stack
- React 15.5.4 (end of life)
- .NET 4.5.2
- Modernize recommended

### Recommended Upgrades
1. **React**: Migrate to v17 or v18
   - Use `react-scripts` or modern webpack
   - Modern lifecycle methods
   
2. **.NET**: Upgrade to .NET Core / .NET 6+
   - Cross-platform capability
   - Modern C# features
   
3. **CefSharp**: Keep current
   - Already good
   - Stay on latest

4. **Redux**: Upgrade to v5+
   - Redux Toolkit
   - Redux DevTools
   
5. **YouTube**: Migrate to official API
   - OAuth 2.0
   - Better rate limits

### Technology Avoidance
- Legacy React (15.5.4)
- Deprecated .NET Framework 4.5.2 (consider 4.8 end-of-life soon)
- Custom streaming (use MQTT/WebRTC for better sync)

## Dependency Management

### Backend Dependencies
Managed via:
- `packages.config` (NuGet)
- Global NuGet cache
- Restore on build

### Frontend Dependencies
Managed via:
- `package.json`
- npm scripts
- webpack bundling

## Compatibility Matrix

| Platform | Backend | Frontend | Notes |
|------|------|------|--------|
| Windows 7+ | ✅ | ✅ | Primary platform |
| Windows 10/11| ✅ | ✅ | Recommended |
| Windows Server| ✅ | ✅ | Server editions |
| Linux | ❌ | ✅ | No WPF, use .NET MAUI |
| macOS | ❌ | ✅ | No WPF, use .NET MAUI |

## License & Legal

### Open Source Components
- React: MIT
- Redux: MIT
- NAudio: BSD
- CefSharp: MIT
- Webpack: MIT
- FontAwesome: SL (free for personal)
- TagLib: LGPL
- SQLite: Public Domain

### YouTube API
- TOS compliance required
- API key restrictions
- Rate limits apply

## Environment Setup

### .NET Environment
```bash
# Minimum requirements
# Windows + .NET Framework 4.5.2
# Visual Studio 2015+ or VS Code with C# extension

# Install dependencies
dotnet restore  # Works on .NET Core
msbuild /t:Restore # .NET Framework
```

### Node Environment
```bash
# Node 8+ (React 15 era requirement)
# npm 5+
npm install
npm run webpack
```

## Technology Summary

**Backend**: .NET Framework 4.5.2 + WPF
**Frontend**: React 15.5.4 + Redux 5 + Webpack
**Audio**: NAudio + TagLib#
**Database**: SQLite via System.Data.SQLite
**YouTube**: CefSharp + YoutubeExplode
**Streaming**: Custom TCP protocol
**Icons**: FontAwesome Free
**Styling**: SCSS → CSS (webpack)
**Build**: MSBuild + Webpack + npm

**Total Size**: ~100+ packages (combined)
**Maintainer Effort**: High (legacy stack + manual streaming)

## Migration Considerations

### If Moving to Modern Stack
1. **.NET**: Switch to .NET 6/8
2. **WPF**: Consider .NET MAUI or Blazor
3. **React**: Upgrade to v17+ LTS
4. **Redux**: Use Redux Toolkit
5. **YouTube**: Official Data API v3
6. **Streaming**: Evaluate MQTT/WS alternatives

### Breaking Changes to Avoid
- Don't touch CefSharp integration (works well)
- Don't change NAudio usage pattern (efficient)
- Keep SQLite (simple, effective)
- Preserve TCP protocol semantics

**Contact** for questions about migration paths or technology questions!
