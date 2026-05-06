# Music Player Architecture

## High-Level Overview

Music Player is a **hybrid music streaming application** built with:

- **Backend**: .NET (C#) using WPF + System.Web + CefSharp
- **Frontend**: React with Redux state management
- **Communication**: Custom JSON-based protocol over TCP for streaming
- **Database**: SQLite for metadata storage
- **Audio Processing**: NAudio library
- **Video Streaming**: CefSharp/Chromium for YouTube

## Technical Stack

### Backend (.NET)
- **Framework**: .NET Framework 4.5.2
- **UI**: WPF (Windows Presentation Foundation)
- **HTTP Server**: System.Web (IIS/ASP.NET)
- **Browser Integration**: CefSharp (Chromium in .NET)

### Key Technologies

| Technology | Purpose |
|------------|---------|
| .NET Framework 4.5.2 | Runtime and base framework |
| NAudio | Audio file reading/playback |
| TagLib# | Audio metadata parsing |
| YoutubeExplode | YouTube API operations |
| CefSharp | Embedded browser for YouTube |
| System.Data.SQLite | Database storage |
| MusicPlayer | Core logic namespace |
| WebAssembly? | Custom streaming protocol over TCP |

See [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md) for full details.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        Music Player System                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                    WPF Application                          │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌───────────────────┐  │  │
│  │  │    MainWindow│  │  MainWindow  │  │  Initialize.ps1   │  │  │
│  │  │      .cs     │  │   .xaml    │  │    (Initialization) │  │  │
│  │  └─────────────┘  └─────────────┘  └───────────────────┘  │  │
│  │                                                             │  │
│  │  MusicPlayer Namespace                                       │  │
│  │  │  Db.cs              │  Factory.cs      │  Interfaces     │  │
│  │  │  Extensions         │  Song/Album/etc. │  IServerContract│  │
│  │  └────────────────────────────────────────────────────────┘   │  │
│  │                                                             │  │
│  │  MusicPlayerGate Namespace                                   │  │
│  │  │  MusicPlayerGate.cs   │  Actions.cs  │  Data.cs         │  │
│  │  │  (Main Entry Point)   │  │            │                  │  │
│  │  └────────────────────────────────────────────────────────┘   │  │
│  │                                                             │  │
│  │  MusicPlayer.Installer Namespace                              │  │
│  │  │  Installer.cs         │  RegistryHelper.cs                 │  │
│  │  └────────────────────────────────────────────────────────┘   │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │              MusicPlayerWeb (CefSharp Browser)             │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │              Startup.cs                              │  │  │
│  │  │           (CefSharp initialization)                  │  │  │
│  │  └─────────────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │       SchemeHandlerFactory.cs                         │  │  │
│  │  │  (Handles file requests from browser)                  │  │  │
│  │  └─────────────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │              Web Resources                           │  │  │
│  │  │    Pages/        │  Resources/         │  Scripts/     │  │  │
│  │  │    index.html    │    icons            │  Helpers/     │  │  │
│  │  └─────────────────────────────────────────────────────┘  │  │
│  │                                                             │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │             React Application (Webpack Bundle)       │  │  │
│  │  │               ┌───────────────────────────────────┐  │  │  │
│  │  │               │        ReduxStore                  │  │  │  │
│  │  │  ┌────────────┼───────────────────────────────────┼────────┐│  │  │
│  │  │  │ Home/      │  │  Playlist                       │  │    ││  │  │
│  │  │  │ Server/    │  │  └─────────────────────────────┬───────┘│  │  │
│  │  │  │ Client/    │  │      │                          │        ││  │  │
│  │  │  │ Copy/      │  │      ▼                          ▼        ││  │  │
│  │  │  │ Video/     │  │  ┌───────────────┐   ┌──────────────┐   ││  │  │
│  │  │  │ Radio/     │  │  │   CurrentSong  │   │ ServerInfo   │   ││  │  │
│  │  │              │  │  │   (Current)     │   │ (Connected)   │   ││  │  │
│  │  │              │  │  └─────────────────┘   └──────────────┘   ││  │  │
│  │  │              │  │                                          │  │  │
│  │  │              │  │      ┌───────────────────────────────────┐│  │  │
│  │  │              │  │      │     CSharpDispatcher.js            ││  │  │
│  │  │              │  │      │  (Bridges C# backend to React)      ││  │  │
│  │  │              │  │      └───────────────────────────────────┘│  │  │
│  │  └──────────────┴──────────────────────────────────────────────┘  │  │
│  │                                                              │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                   │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                  External Integrations                       │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

Key Communication Flows:

1. User Action → React Component → Redux State → CSharpDispatcher → C# Backend
2. C# Backend → WebAssembly/Server → Real-time data to React
3. TCP Streaming Protocol: Client ↔ Server (port ~8963)
4. HTTP Requests: Browser → React Web Server (ASP.NET)
```

## Component Breakdown

### 1. WPF Application Layer
- **Purpose**: Main desktop application entry point
- **Files**:
  - `App.xaml.cs` - Application startup
  - `MainWindow.xaml.cs` - Main window logic
  - `MusicPlayerGate.` - Controls and orchestration

### 2. Music Player Core
- **Purpose**: Audio playback and library management
- **Key Classes**:
  - `Db` - SQLite database operations
  - `Factory` - Object creation singleton
  - `Song/Album/Artist/Playlist` - Entity models

### 3. MusicPlayerWeb (CefSharp Browser)
- **Purpose**: React-based web interface embedded in CefSharp
- **Purpose**: 
  - Host music server
  - Connect to remote servers
  - Stream YouTube videos
  - Internet radio browsing
- **Architecture**:
  - CefSharp initializes browser process
  - Startup.cs registers custom schemes
  - SchemeHandlerFactory serves static files
  - Webpack builds React bundle to `bin/x64/Debug/Web/Scripts/Build/bundle.js`

### 4. Streaming Protocol
- **Purpose**: Real-time audio/video streaming
- **Protocol**: Custom TCP-based protocol (WebAssembly-compatible)
- **Ports**: ~8963 (configurable)
- **Mechanism**: 
  - Server broadcasts audio to connected clients
  - YouTube sync maintained across clients
  - Volume controls synchronized

### 5. Data Stores (Redux)
See [HOW-IT-WORKS.md](./HOW-IT-WORKS.md#redux-state-management) for details on the three main state slices:
- `currentState` - Current song/video playing
- `ServerInfo` - Server connection info (clients, status, current video)
- `CopyProgress` - File copying progress

## Data Models

### Song Entity
```typescript
{
  // Audio track metadata
  Title: string,
  Artist: string,
  Album: string,
  Genre: string,
  Path: string,
  Duration: number, // in milliseconds
  Bitrate: number,  // in kbps
  // ... additional metadata from taglib
}
```

### ServerInfo
```typescript
{
  IsHost: boolean,        // true if running as server
  Host: string,           // Server IP/host
  Port: number,           // Server port
  Clients: Record<string, string>, // {IP: "port" entries}
  VideoUrl: string | null,  // Current YouTube video
  VideoPosition: number   // Current video position
}
```

See [API.md](./API.md) for full data model definitions.

## File Structure

```
MusicPlayer/
├── MusicPlayer.csproj           # WPF project
├── MusicPlayer.Installer/       # Setup installer
├── packages.config              # NuGet packages
├── Resources/
│   ├── DirbleApiKey.dirbleapikey
│   ├── ReadMe.txt
│   └── ...
├── MusicPlayerWeb/             # CefSharp + Web
│   ├── MusicPlayerWeb.csproj   # CefSharp/ASP.NET project
│   ├── Startup.cs             # Browser init
│   ├── SchemeHandlerFactory.cs
│   ├── redist/                # Dependencies
│   └── Web/                   # React frontend
│       ├── Pages/
│       ├── Scripts/
│       ├── Style/
│       └── Resources/
└── [Music library folders]
```

## Build & Deployment

### Build
```bash
# Backend (WPF)
# Requires Visual Studio + .NET Framework 4.5.2

# Frontend (React)
cd MusicPlayerWeb/Web
npm install
npm run webpack
```

### Deploy
1. Package installer via MusicPlayer.Installer
2. Deploy WPF app + MusicPlayerWeb
3. Run Initialize.ps1
4. Configure port (default: 8963)
5. Open app to host or connect

### Dependencies
- .NET Framework 4.5.2
- Visual Studio (for development)
- NAudio, TagLib#, YoutubeExplode, CefSharp
- SQLite

## See Also

- [FEATURES.md](./FEATURES.md) - Complete feature list
- [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md) - Technology details
- [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) - Detailed workflows
- [API.md](./API.md) - Data models and endpoints
