# Music Player Documentation Index

# Overview [TOC]

Welcome to the **Music Player Documentation** for the current state of this repository.

This project is a .NET-based music streaming application that combines:
- A **WPF desktop backend** (C# / .NET Framework 4.5.2)
- A **React web frontend** (v15.5.4) embedded via CefSharp
- Custom **TCP streaming protocol** for real-time audio broadcasting
- Full **YouTube integration** for video streaming
- Support for **internet radio** and **file copying utilities**

---

# Quick Start

## What Is This App?

Music Player is a desktop music streaming application that lets you:
- **Host** a music server to stream audio to other devices
- **Connect** to remote servers as a client
- **Stream YouTube** videos with all clients synchronized
- **Browse** internet radio stations
- **Copy** random files between folders
- **Load** large music libraries efficiently

## Current State

This documentation captures the **current state** of the repository. As changes are made, relevant documentation sections will be updated accordingly.

## Quick Links

| Document | Description |
|---------|-------------|
| [Architecture](./ARCHITECTURE.md) | High-level system architecture, components, and data flow |
| [Features](./FEATURES.md) | Complete feature list and usage guide |
| [Technology Stack](./TECHNOLOGY-STACK.md) | Technologies, libraries, and dependencies |
| [How It Works](./HOW-IT-WORKS.md) | Detailed inner workings of features |
| [API Reference](./API.md) | Data structures, functions, and protocols |
| [Index](./README.md) | This overview |

---

# Documentation Summary

## Architecture Overview

The application follows a **hybrid architecture**:

```
┌────────────────────────────────────────────────────────────┐
│                      Music Player System                     │
├────────────────────────────────────────────────────────────┤
│                                                              │
│  Desktop App (WPF)                                          │
│    └── MainWindow.xaml                                        │
│        └── MusicPlayer namespace                            │
│            └── Audio playback, library management          │
│                                                              │
│  Browser (CefSharp)                                         │
│    └── React App (Webpack Bundle)                           │
│        └── Redux Store                                        │
│            ├── currentSong                                   │
│            ├── serverInfo                                    │
│            └── copyProgress                                 │
│                                                              │
│  Streaming (Custom TCP Protocol)                            │
│    └── Server ↔ Client communication                        │
│                                                              │
└────────────────────────────────────────────────────────────┘
```

**See [ARCHITECTURE.md](./ARCHITECTURE.md) for full details.**

## Key Features

- ✅ **Music Library** - Support for large audio libraries
- ✅ **Server Mode** - Host music to multiple clients
- ✅ **Client Mode** - Connect to remote streams
- ✅ **YouTube Streaming** - Play and sync videos
- ✅ **Playlists** - Sync playlists across clients
- ✅ **Internet Radio** - Browse and tune in
- ✅ **File Copy** - Copy random files between folders
- ✅ **Metadata** - Full ID3/TagLib metadata support

**See [FEATURES.md](./FEATURES.md) for feature details.**

## Technology Stack

**Backend:**
- .NET Framework 4.5.2
- WPF + System.Web
- CefSharp

**Frontend:**
- React 15.5.4
- Redux 5
- Webpack

**Libraries:**
- NAudio (audio)
- TagLib# (metadata)
- YoutubeExplode (YouTube)
- SQLite (database)

**See [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md) for full list.**

## How It Works

The application uses:
- **Redux** for state management
- **CSharpDispatcher** for JS-interop with C#
- **Custom TCP protocol** for streaming
- **SQLite** for metadata storage

Major flows documented:
1. **Startup** → WPF + CefSharp initialization
2. **Audio Import** → NAudio scanning
3. **Server Connection** → TCP handshake
4. **YouTube Sync** → Position/volume sync
5. **State Updates** → Redux dispatch flow

**See [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) for detailed flows.**

## API Documentation

The API covers:
- **C# Bridge** - Functions exposed to JavaScript
- **Redux State** - State shapes and actions
- **Streaming Protocol** - TCP message format
- **YouTube API** - Integration patterns
- **Database API** - SQLite operations

**See [API.md](./API.md) for full API reference.**

---

# Current Repository State

## Project Structure

```
.
├── MusicPlayer/                    # WPF backend project
│   ├── MusicPlayer.csproj
│   ├── MusicPlayerGate.*.cs
│   └── [audio logic]
├── MusicPlayer.Installer/          # Setup installer
├── MusicPlayerWeb/                 # CefSharp + Web project
│   ├── MusicPlayerWeb.csproj
│   ├── Startup.cs
│   ├── SchemeHandlerFactory.cs
│   └── Web/                       # React frontend
│       ├── Pages/
│       ├── Scripts/
│       ├── Style/
│       └── Resources/
└── README.md
```

## Status

| Aspect | Status | Notes |
|--------|--------|-|-|
| Functionality | ✅ Operational | All features working |
| Code State | ⚠️ Legacy | React 15.5.4, .NET 4.5.2 |
| Documentation | ✅ Complete | Current state documented |
| Build System | ⚠️ Visual Studio 2015+ | .NET Framework |

## Known Limitations

1. **React Version**: v15.5.4 (very outdated, consider upgrade)
2. **.NET Framework**: 4.5.2 (modernize recommended)
3. **Protocol**: Custom TCP (consider HTTP/WebSocket alternatives)
4. **Platform**: Windows-only (WPF limitation)

## Upgrade Opportunities

- **React v16/v17/v18** migration
- **.NET Core / .NET 6+** for cross-platform
- **Official YouTube Data API** instead of embed
- **HTTPS** support
- **WebSocket** for modern streaming

---

# Contributing

## When Making Changes

When you modify the codebase, update this documentation:

- **Architecture changed** → [ARCHITECTURE.md](./ARCHITECTURE.md)
- **New feature added** → [FEATURES.md](./FEATURES.md)
- **Library dependency added** → [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md)
- **New module/flow documented** → [HOW-IT-WORKS.md](./HOW-IT-WORKS.md)
- **New API endpoint/function** → [API.md](./API.md)

## Build Instructions

### Build Backend (WPF)
```bash
# Requires Visual Studio + .NET Framework 4.5.2
msbuild MusicPlayer.sln /p:Configuration=Release
```

### Build Frontend (React)
```bash
cd MusicPlayerWeb/Web
npm install
npm run webpack
```

### Run
```bash
# Backend
MusicPlayer.exe

# Frontend (auto-loaded by CefSharp)
```

---

# Documentation Maintenance Guidelines

For detailed guidelines on maintaining project documentation, including update triggers, file management rules, and verification steps, refer to the **Music Player Docs Maintenance** skill:

- **Skill location**: `.hermes/skills/music-player/music-player-docs-maintenance/SKILL.md`
- **Skill name**: `music-player-docs-maintenance` (load with `skill_view(name='music-player-docs-maintenance')`)

### Quick Rules
1. Every code change requires documentation updates
2. Update `docs/INDEX.md` TOC when adding new docs
3. Verify documentation status as the last step of any task

---

# Version History

| Version | Date | Notes |
|--------|------|-------|
| Initial | N/A | Current state documented |

---

# License & Attribution

- **Music Player**: See repository LICENSE
- **React**: MIT
- **CefSharp**: MIT
- **NAudio**: BSD
- **TagLib#**: LGPL
- **YouTube**: Subject to YouTube Terms of Service

---

# Quick Reference

### Most Common Tasks

| Task | How-To | See |
|------|--------|-----|
| Import music | Click "Open Folder" | [Features](./FEATURES.md) |
| Host server | `/server` → Set port → "Host" | [Features](./FEATURES.md) |
| Connect client | `/client` → Enter IP:port → "Connect" | [Features](./FEATURES.md) |
| Play YouTube | `/video` → Paste URL → "Play" | [Features](./FEATURES.md) |
| Browse radio | `/radio` → Search → Click station | [Features](./FEATURES.md) |
| Copy files | `/copy` → Select folders → "Copy" | [Features](./FEATURES.md) |

---

# Support & Troubleshooting

**Common issues:**

- Videos not playing → Check YouTube API/iframe
- Can't connect → Verify IP/port/firewall
- Slow import → Large libraries may need batching

**See [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) for troubleshooting.**

---

**End of documentation overview.**

For detailed information on any aspect, see the relevant documentation files above.
