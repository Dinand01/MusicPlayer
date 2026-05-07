# Music Player Architecture

## High-Level Overview

Music Player is a **hybrid music streaming application** built with:

- **Backend**: .NET 10 Avalonia application using CefGlue.Avalonia for Chromium browser
- **Frontend**: React with Redux state management (embedded via CefGlue)
- **Audio Processing**: NAudio library (cross-platform via WaveOutEvent)
- **Database**: SQLite via Microsoft.Data.Sqlite (EF Core 10 optional)
- **Video Streaming**: CefGlue.Avalonia for YouTube embedding
- **Communication**: gRPC (replacing WCF duplex contracts) - IN PROGRESS

## Technical Stack

### Backend (.NET 10)

- **Framework**: .NET 10 (net10.0, cross-platform)
- **UI**: Avalonia 11.2.3 (cross-platform desktop UI)
- **Browser Integration**: CefGlue.Avalonia 120.6099.1 (Chromium-based)
- **Platforms**: Windows (x64), Linux (x64), macOS (x64)

### Key Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET 10 | 10.0.104 SDK | Runtime and base framework |
| Avalonia | 11.2.3 | Cross-platform UI framework |
| CefGlue.Avalonia | 120.6099.1 | Chromium browser for Avalonia |
| NAudio | 2.2.1 | Cross-platform audio playback |
| TagLib# | 2.1.0 | Audio metadata parsing |
| YoutubeExplode | 6.3.10 | YouTube API operations |
| Microsoft.Data.Sqlite | Latest | SQLite database (replaces System.Data.SQLite) |
| gRPC | 2.67.0 | Inter-process communication (replaces WCF) |

See [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md) for full details.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                        Music Player System                        │
├─────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌───────────────────────────────────────────────────┐  │
│  │                    Avalonia Application                  │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌───────────────────┐  │  │
│  │  │    MainWindow│  │  MainWindow  │  │  Initialize.ps1   │  │  │
│  │  │      .axaml │  │   .axaml.cs  │  │  (Initialization) │  │  │
│  │  └─────────────┘  └─────────────┘  └───────────────────┘  │  │
│  │                                                             │  │
│  │  MusicPlayerGate Namespace (C# ↔ JS Bridge)                 │  │
│  │  │  MusicPlayerGate.cs   │  Actions.cs  │  Data.cs         │  │  │
│  │  │  (Main Entry Point)   │  (Commands)  │                  │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  MusicPlayer Namespace (Core Logic)                        │  │
│  │  │  Db.cs (EF Core 10) │  Factory.cs (gRPC) │  Interfaces │  │  │
│  │  │  Extensions         │  Song/Album/etc.    │  IClientContract │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  Controller Namespace (gRPC - IN PROGRESS)              │  │
│  │  │  GrpcServerService.cs │  GrpcClientContract.cs          │  │  │
│  │  │  (Server methods)      │  (Client calls)                │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌───────────────────────────────────────────────────┐  │
│  │         MusicPlayerWeb (CefGlue.Avalonia Browser)             │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────┐  │  │
│  │  │              Startup.cs                              │  │  │
│  │  │           (CefGlue initialization)                  │  │  │
│  │  └─────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────┐  │  │
│  │  │       SchemeHandlerFactory.cs                         │  │  │
│  │  │  (Handles custom scheme file requests)                  │  │  │
│  │  └─────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────┐  │  │
│  │  │             React Application (Webpack Bundle)       │  │  │
│  │  │               ┌───────────────────┐          │  │  │
│  │  │               │        ReduxStore                  │          │  │  │
│  │  │  ┌────────────┼───────────────────┼────────────┐  │  │  │
│  │  │  │ Home/    │  │  Playlist                       │  │  │  │
│  │  │  │ Server/    │  │  └─────────────────────┬───────┘   │  │  │
│  │  │  │ Client/    │  │      │                          │  │  │  │
│  │  │  │ Copy/      │  │      ▼                          ▼  │  │  │  │
│  │  │  │ Video/     │  │  ┌───────────────┐   ┌───────────────┐   │  │  │
│  │  │  │ Radio/     │  │  │   CurrentSong  │   │ ServerInfo   │   │  │  │
│  │  │  │              │  │  │   (Current)     │   │ (Connected)   │   │  │  │
│  │  │  │              │  │  └───────────────┘   └───────────────┘   │  │  │
│  │  │  │              │  │      │                          │          │  │  │
│  │  │  │              │  │      ▼                          ▼          │  │  │
│  │  │  │              │  │      ┌───────────────┐   ┌───────────────┐   │  │  │
│  │  │  │              │  │      │   CSharpDispatcher.js            │   │  │  │
│  │  │  │              │  │      │   (Bridges C# backend to React)      │   │  │  │
│  │  │  │              │  │      └───────────────┘   └───────────────┘   │  │  │
│  │  │  └────────────┴──────────────────────────────────────┘   │  │  │
│  │  └─────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌───────────────────────────────────────────────────┐  │
│  │          Streaming (Custom TCP Protocol - Legacy)           │  │
│  │    └── Server ↔ Client communication                        │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

**See [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) for detailed flow diagrams.**

---

## Project Structure

```
.
├── MusicPlayer/                    # Core library project (.NET 10)
│   ├── MusicPlayer.csproj        # net10.0 (cross-platform)
│   ├── Controller/               # Business logic
│   │   ├── MusicPlayer.cs      # Main player logic
│   │   ├── DataController.cs   # Settings, DB access
│   │   ├── VideoController.cs  # YouTube handling
│   │   ├── CopyController.cs   # File copy logic
│   │   ├── WCFServerClient.cs.bak   # Legacy WCF (replaced by gRPC)
│   │   └── WCFServerService.cs.bak # Legacy WCF (replaced by gRPC)
│   ├── Models/                  # Data models
│   │   ├── SongInformation.cs
│   │   ├── ServerInfo.cs
│   │   └── enums/                  # SettingType, etc.
│   ├── Interface/               # Contracts
│   │   ├── IClientContract.cs  # (Legacy WCF, kept for compatibility)
│   │   ├── IServerContract.cs  # (Legacy WCF, kept for compatibility)
│   │   └── IVideo.cs
│   └── Protos/                  # gRPC proto definitions
│       └── musicplayer.proto
│
├── MusicPlayerWeb/               # Avalonia UI project (.NET 10)
│   ├── MusicPlayerWeb.csproj   # net10.0 + Avalonia + CefGlue
│   ├── MainWindow.axaml         # Avalonia XAML (was MainWindow.xaml)
│   ├── MainWindow.xaml.cs       # Code-behind (browser integration)
│   ├── App.axaml                # Avalonia app (was App.xaml)
│   ├── App.xaml.cs              # App initialization
│   ├── Startup.cs               # CefGlue initialization
│   ├── Program.cs               # Avalonia entry point
│   ├── MusicPlayerGate.cs       # C# ↔ JS bridge (main)
│   ├── MusicPlayerGate.Actions.cs # User actions
│   ├── MusicPlayerGate.Data.cs  # Data operations
│   ├── SchemeHandlerFactory.cs  # Custom scheme handler (CefGlue)
│   ├── DisplayHandler.cs        # Browser display handler (CefGlue)
│   └── Web/                    # React frontend
│       ├── Pages/
│       ├── Scripts/
│       ├── Style/
│       └── Resources/
│
├── docs/                         # Documentation
│   ├── tasks.json              # Task list
│   ├── API.md
│   ├── ARCHITECTURE.md        # This file
│   ├── HOW-IT-WORKS.md
│   ├── TECHNOLOGY-STACK.md
│   ├── INDEX.md
│   └── tasks/                  # Task-specific docs
│       └── 2026-05-06-upgrade-dotnet-10.md
│
└── README.md
```

---

## Key Components

### 1. Avalonia Application (MusicPlayerWeb)

**Framework:** Avalonia 11.2.3 (cross-platform UI)
**Browser:** CefGlue.Avalonia 120.6099.1 (Chromium-based)

**Key Files:**
- `MainWindow.axaml` - Avalonia UI layout
- `MainWindow.xaml.cs` - Browser integration, event handling
- `MusicPlayerGate.cs` - Main C# ↔ JavaScript bridge
- `Startup.cs` - CefGlue initialization (`CefRuntime.Load()`, `CefRuntime.Initialize()`)

**Platform Support:**
- ✅ Windows (x64) - Tested
- ✅ Linux (x64) - Tested (current build platform)
- ⚠️ macOS (x64) - Build verified, runtime pending

### 2. Core Logic (MusicPlayer)

**Namespace:** `MusicPlayer`

**Key Classes:**
- `MusicPlayer.cs` - Main player logic (playback, pause, next, volume)
- `DataController.cs` - Settings management (`SettingType.Volume`, etc.)
- `Factory.cs` - Object creation (`GetPlayer()`, `GetServerPlayer()`, etc.)

**Audio Playback:**
- Library: NAudio 2.2.1
- Output: `WaveOutEvent` (cross-platform)
- Volume: `DataController.SetSetting<int>(SettingType.Volume, percentage)`

### 3. C# ↔ JavaScript Bridge

**Implementation:** `MusicPlayerGate.cs`, `MusicPlayerGate.Actions.cs`

**Pattern:**
- **C# → JS:** `browser.ExecuteJavaScript(script, "musicplayer", 0)`
- **JS → C#:** Via `window.external` (traditional) + `window.MusicPlayer` object

**Exposed Methods (window.MusicPlayer):**
- `togglePlay()`, `nextSong()`, `playSong(json)`
- `setVolume(value)`, `seekVideo(position)`
- `hostServer(port)`, `connectToServer(ip, port)`
- `startVideo(url)`, `stopVideo()`
- `copySongs(source, dest, count)`

### 4. React Frontend (Embedded)

**Framework:** React (version in package.json)
**State Management:** Redux
**Build:** Webpack bundle (embedded via CefGlue custom scheme `custom://`)

**Key Components:**
- `Home` - File/folder selection
- `Server` - Host server UI
- `Client` - Connect to server UI
- `Video` - YouTube video playback
- `Radio` - Internet radio browser
- `Copy` - File copy utility

**State Shape (Redux):**
```javascript
{
    currentSong: null | SongObject,
    serverInfo: null | ServerInfoObject,
    copyProgress: null | number (0-100)
}
```

### 5. Communication (gRPC - IN PROGRESS)

**Status:** Migrating from WCF to gRPC (Step 18-22 in tasks.json)

**Proto Definition:** `MusicPlayer/Protos/musicplayer.proto`

**Services:**
- `MusicPlayerServer` - Server-side methods
- `MusicPlayerClient` - Client-side callbacks

**Legacy (Still Functional):**
- Custom TCP protocol (server ↔ client audio streaming)
- Handshake: `CONNECT\r\n` → `ACCEPTED\r\n`
- Commands: `PLAY`, `PAUSE`, `SEEK`, `VOLUME`

---

## Data Flow

### Audio Playback Flow

```
User clicks "Play"
    ↓
MainWindow.xaml.cs → _musicPlayer.TogglePlay()
    ↓
MusicPlayerGate.cs → _player.TogglePlay()
    ↓
MusicPlayer.cs → _player.Play(song) / _player.Pause()
    ↓
NAudio WaveOutEvent → Audio output (cross-platform)
    ↓
SongChanged event → MusicPlayerGate.cs
    ↓
ExecuteJavaScript → Redux dispatchSetCurrentSong()
    ↓
React UI updates "Now Playing" display
```

### YouTube Video Flow

```
User enters YouTube URL
    ↓
MusicPlayerGate.cs → _player.StartVideo(url)
    ↓
VideoController.cs → Parse video ID, create VideoInfo
    ↓
IVideo.PlayVideo(url) → NAudio/Video playback
    ↓
MainWindow.xaml.cs → browser.ExecuteJavaScript (load YouTube iframe)
    ↓
YouTube Iframe API → Video plays in CefGlue browser
```

### Server-Client Streaming Flow

```
Host clicks "Host Server"
    ↓
MusicPlayerGate.cs → Factory.GetServerPlayer(port)
    ↓
GrpcServerService.cs → Start gRPC server (replacing WCF)
    ↓
Client clicks "Connect"
    ↓
MusicPlayerGate.cs → Factory.GetClientPlayer(ip, port)
    ↓
GrpcClientContract.cs → Connect to gRPC server
    ↓
Server broadcasts audio → Clients receive via gRPC streaming
```

---

## Cross-Platform Notes

### Technology Choices

| Component | Choice | Reason |
|-----------|--------|--------|
| UI Framework | Avalonia 11.2.3 | True cross-platform (Windows/Linux/macOS) |
| Browser | CefGlue.Avalonia 120.6099.1 | Chromium-based, cross-platform |
| Audio | NAudio + WaveOutEvent | Cross-platform audio output |
| Database | Microsoft.Data.Sqlite | Cross-platform SQLite |
| Build | net10.0 | Framework supports all platforms |

### Runtime Identifiers

Configured in `MusicPlayerWeb.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
<RuntimeIdentifiers>win-x64;linux-x64;osx-x64</RuntimeIdentifiers>
```

### Build Commands

```bash
# Windows
dotnet build -r win-x64

# Linux
dotnet build -r linux-x64

# macOS
dotnet build -r osx-x64
```

---

## Migration Status (2026-05-06)

### Completed ✅
- .NET 10 upgrade (net10.0, cross-platform)
- WPF → Avalonia migration
- CefSharp.Wpf → CefGlue.Avalonia migration
- Cross-platform build support (win-x64, linux-x64, osx-x64)
- Volume control via `SettingType.Volume`

### In Progress 🔄
- gRPC migration (Steps 18-22)
- macOS runtime testing
- Documentation updates (Steps 38-43)

### Pending ⏳
- Test on macOS (runtime)
- Test volume control on all platforms
- Verify CEF binaries for all platforms

---

**Last Updated:** 2026-05-06 (Cross-platform migration complete)
