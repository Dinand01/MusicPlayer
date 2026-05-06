# Music Player Architecture

## High-Level Overview

Music Player is a **hybrid music streaming application** built with:

- **Backend**: .NET 10 WPF using gRPC for communication
- **Frontend**: React with Redux state management
- **Browser Engine**: CefSharp (Chromium embedded) - Windows only
- **Audio Processing**: NAudio library
- **Database**: SQLite via EF Core 10
- **Video Streaming**: CefSharp/Chromium for YouTube
- **Communication**: gRPC (replacing WCF duplex contracts)

## Technical Stack

### Backend (.NET 10)
- **Framework**: .NET 10 (net10.0-windows for WPF)
- **UI**: WPF (Windows Presentation Foundation)
- **Communication**: gRPC (Google Remote Procedure Call)
- **Browser Integration**: CefSharp (Chromium in .NET, Windows only)

### Key Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET 10 | 10.0.104 SDK | Runtime and base framework |
| NAudio | 2.2.1 | Audio file reading/playback |
| TagLib# | 2.1.0 | Audio metadata parsing |
| YoutubeExplode | 6.3.10 | YouTube API operations |
| CefSharp | 128.4.90 | Embedded browser for YouTube (Windows) |
| EF Core | 10.0.0 | Database ORM (replaces EF6) |
| gRPC | 2.67.0 | Inter-process communication (replaces WCF) |
| MusicPlayer | Core logic namespace |
| MusicPlayerWeb | Browser bridge (CefSharp) |

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
│  │  │  Db.cs (EF Core 10) │  Factory.cs (gRPC) │  Interfaces │  │
│  │  │  Extensions         │  Song/Album/etc.    │  IClientContract │  │
│  │  └────────────────────────────────────────────────────────┘   │  │
│  │                                                             │  │
│  │  MusicPlayerGate Namespace                                   │  │
│  │  │  MusicPlayerGate.cs   │  Actions.cs  │  Data.cs         │  │
│  │  │  (Main Entry Point)   │  (Commands)  │                  │  │
│  │  └────────────────────────────────────────────────────────┘   │  │
│  │                                                             │  │
│  │  Controller Namespace (gRPC)                                 │  │
│  │  │  GrpcServerService.cs │  GrpcClientContract.cs          │  │
│  │  │  (Server methods)      │  (Client calls)                │  │
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
│  │  │             React Application (Webpack Bundle)       │  │  │
│  │  │               ┌───────────────────────────┐          │  │  │
│  │  │               │        ReduxStore                  │  │  │  │
│  │  │  ┌────────────┼───────────────────────────┼────────┐│  │  │
│  │  │  │ Home/      │  │  Playlist                       │  │    ││  │  │
│  │  │  │ Server/    │  │  └─────────────────────┬───────┘│  │  │
│  │  │  │ Client/    │  │      │                          │        ││  │  │
│  │  │  │ Copy/      │  │      ▼                          ▼        ││  │  │
│  │  │  │ Video/     │  │  ┌───────────────┐   ┌──────────────┐   ││  │  │
│  │  │  │ Radio/     │  │  │   CurrentSong  │   │ ServerInfo   │   ││  │  │
│  │  │  │              │  │  │   (Current)     │   │ (Connected)   │   ││  │  │
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
```

Key Communication Flows:

1. User Action → React Component → Redux State → CSharpDispatcher → C# Backend
2. C# Backend → gRPC Server → gRPC Client → React (via CefSharp JS interop)
3. gRPC Communication: Server ↔ Client (replaces WCF duplex)
4. HTTP Requests: Browser → React → YouTube API

## Component Breakdown

### 1. WPF Application Layer
- **Purpose**: Main desktop application entry point
- **Files**:
  - `App.xaml.cs` - Application startup
  - `MainWindow.xaml.cs` - Main window logic
  - `MusicPlayerGate.*` - Controls and orchestration

### 2. Music Player Core
- **Purpose**: Audio playback and library management
- **Key Classes**:
  - `Db` - SQLite database operations (EF Core 10)
  - `Factory` - Object creation, gRPC server/client setup
  - `Song/Album/Artist/Playlist` - Entity models
  - `MusicPlayer` - Core playback logic (NAudio)

### 3. gRPC Communication Layer
- **Purpose**: Replace WCF duplex contracts for client-server communication
- **Key Files**:
  - `Protos/musicplayer.proto` - Protocol buffer definitions
  - `GrpcServerService.cs` - Server-side gRPC methods
  - `GrpcClientContract.cs` - Client-side gRPC calls
  - `IClientContract.cs` / `IServerContract.cs` - Interface contracts

### 4. MusicPlayerWeb (CefSharp Browser)
- **Purpose**: React-based web interface embedded in CefSharp
- **Purpose**: 
  - Host music server
  - Connect to remote servers via gRPC
  - Stream YouTube videos
  - Internet radio browsing
- **Architecture**:
  - CefSharp initializes browser process
  - Startup.cs registers custom schemes
  - SchemeHandlerFactory serves static files
  - Webpack builds React bundle to `bin/x64/Debug/Web/Scripts/Build/bundle.js`
- **Platform Limitation**: Windows only (CefSharp.Wpf 128.4.90 doesn't support net10.0-windows on Linux)

### 5. Data Layer (EF Core 10)
- **Purpose**: SQLite database with code-first ORM
- **Technology**: EF Core 10 with Microsoft.Data.Sqlite
- **Key Files**:
  - `Db.cs` - Database context and repositories
  - Entity classes: `Song`, `Album`, `Artist`, `Playlist`, `Genre`

### 6. Audio Processing
- **Purpose**: Audio playback and metadata
- **Technology**: NAudio 2.2.1 + TagLib# 2.1.0
- **Key Files**:
  - `MusicPlayer.cs` - Core playback logic
  - `VideoController.cs` - YouTube video handling (YoutubeExplode 6.3.10)

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
  Port: number,           // Server port (gRPC)
  Clients: Record<string, string>, // {IP: "port" entries}
  VideoUrl: string | null,  // Current YouTube video
  VideoPosition: number   // Current video position
}
```

See [API.md](./API.md) for full data model definitions.

## gRPC Communication

### Protocol Buffers (musicplayer.proto)
Defines two services:
1. **MusicPlayerServerService** - Methods called by client on server:
   - `Anounce` - Client announcement
   - `Goodbye` - Client disconnection
   - `GetCurrentPosition` - Get playback position

2. **MusicPlayerClientService** - Methods called by server on client:
   - `PlayVideo`, `SeekVideo`, `SetSongPosition`, `SetSong`
   - `SendFile`, `Play`, `PlayRadio`, `Pause`, `Disconnect`

### gRPC vs WCF
| Feature | WCF (Old) | gRPC (New) |
|---------|-----------|------------|
| Duplex contracts | NetTcpBinding | Bidirectional streaming |
| Serialization | XML/DataContract | Protocol Buffers |
| Platform support | Windows-only | Cross-platform |
| Performance | Slower | Faster (binary) |

## File Structure

```
MusicPlayer/
├── MusicPlayer.csproj           # WPF project (.NET 10)
├── MusicPlayerWeb.csproj       # CefSharp/Web project (Windows)
├── MusicPlayer.Installer/       # Setup installer
├── Protos/
│   └── musicplayer.proto       # gRPC protocol definitions
├── Controller/                  # gRPC services
│   ├── GrpcServerService.cs
│   ├── GrpcClientContract.cs
│   └── MusicPlayer.cs          # Playback logic
├── Models/                      # Entity classes
│   ├── Song.cs, Album.cs, etc.
│   └── VideoInfo.cs
├── Factory.cs                   # Singleton + gRPC setup
├── Db.cs                        # EF Core 10 database
├── MusicPlayerWeb/             # CefSharp + Web
│   ├── Startup.cs             # Browser init
│   ├── SchemeHandlerFactory.cs
│   └── Web/                   # React frontend
│       ├── Pages/
│       ├── Scripts/
│       └── Resources/
└── [Music library folders]
```

## Build & Deployment

### Build
```bash
# Backend (WPF) - Windows or Linux
dotnet build MusicPlayer/MusicPlayer.csproj

# Frontend (React)
cd MusicPlayerWeb/Web
npm install
npm run webpack

# Full solution (Windows only - CefSharp)
dotnet build MusicPlayer.sln
```

### Deploy
1. Package installer via MusicPlayer.Installer
2. Deploy WPF app + MusicPlayerWeb (Windows)
3. Run Initialize.ps1
4. Configure gRPC port (default: 5000-5001)
5. Open app to host or connect

### Dependencies
- .NET 10 SDK
- Visual Studio 2022+ (for development)
- NAudio, TagLib#, YoutubeExplode, CefSharp (Windows)
- EF Core 10 + Microsoft.Data.Sqlite
- gRPC packages (Grpc.AspNetCore, Grpc.Net.Client)

## Migration Notes

### From .NET Framework 4.5.2 to .NET 10
- **WCF → gRPC**: Duplex contracts replaced with gRPC services
- **EF6 → EF Core 10**: Database ORM upgraded
- **System.Data.SQLite → Microsoft.Data.Sqlite**: Provider changed
- **Packages**: All upgraded to .NET 10 compatible versions

### Known Issues
- **CefSharp**: Windows-only, blocked on Linux for net10.0-windows
- **TagLib#**: NU1701 warning (restored using .NET Framework)
- **YoutubeExplode**: Major API changes in 6.x (IAsyncEnumerable, VideoId)

## See Also

- [FEATURES.md](./FEATURES.md) - Complete feature list
- [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md) - Technology details
- [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) - Detailed workflows
- [API.md](./API.md) - Data models and endpoints
