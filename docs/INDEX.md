# Music Player Documentation Index

# Overview [TOC]

Welcome to the **Music Player Documentation** for the current state of this repository.

This project is a .NET-based music streaming application that combines:
- A **Avalonia desktop backend** (C# / .NET 10)
- A **React web frontend** embedded via CefGlue.Avalonia (Chromium)
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

This documentation captures the **current state** of the repository (migrated to .NET 10 with cross-platform support). As changes are made, relevant documentation sections will be updated accordingly.

## Quick Links

| Document | Description |
|---------|-------------|
| [Architecture](./ARCHITECTURE.md) | High-level system architecture, components, and data flow |
| [Features](./FEATURES.md) | Complete feature list and usage guide |
| [Technology Stack](./TECHNOLOGY-STACK.md) | Technologies, libraries, and dependencies |
| [How It Works](./HOW-IT-WORKS.md) | Detailed inner workings of features |
| [API Reference](./API.md) | Data structures, functions, and protocols |
| [Task List](./tasks.json) | Current development tasks and progress |

---

# Documentation Summary

## Architecture Overview

The application follows a **hybrid architecture**:

```
┌────────────────────────────────────────────────────┐
│                      Music Player System                     │
├────────────────────────────────────────────────────┤
│                                                              │
│  Desktop App (Avalonia + CefGlue)                       │
│    └── MainWindow.axaml                                  │
│        └── MusicPlayerGate namespace                      │
│            ├── Audio playback, library management          │
│            └── CefGlue.Avalonia browser integration       │
│                                                              │
│  Browser (CefGlue.Avalonia - Chromium)                         │
│    └── React App (Webpack Bundle)                           │
│        └── Redux Store                                        │
│            ├── currentSong                                   │
│            ├── serverInfo                                    │
│            └── copyProgress                                 │
│                                                              │
│  Streaming (Custom TCP Protocol)                            │
│    └── Server ↔ Client communication                        │
│                                                              │
└────────────────────────────────────────────────────┘
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
- ✅ **Cross-Platform** - Windows, Linux, macOS support

**See [FEATURES.md](./FEATURES.md) for feature details.**

## Technology Stack

**Backend:**
- .NET 10 (net10.0)
- Avalonia 11.2.3 (cross-platform UI)
- CefGlue.Avalonia 120.6099.1 (Chromium browser)

**Frontend:**
- React (version in package.json)
- Redux
- Webpack

**Libraries:**
- NAudio (cross-platform audio)
- TagLib# (metadata)
- YoutubeExplode (YouTube)
- Microsoft.Data.Sqlite (database)

**See [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md) for full list.**

## How It Works

The application uses:
- **Redux** for state management
- **window.MusicPlayer** for JS-interop with C#
- **Custom TCP protocol** for streaming
- **SQLite** for metadata storage
- **CefGlue.Avalonia** for Chromium browser integration

Major flows documented:
1. **Startup** → Avalonia + CefGlue initialization
2. **Audio Import** → NAudio scanning
3. **Server Connection** → TCP handshake
4. **YouTube Sync** → Position/volume sync
5. **State Updates** → Redux dispatch flow

**See [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) for detailed flows.**

## API Documentation

The API covers:
- **C# Bridge** - Functions exposed to JavaScript via window.MusicPlayer
- **Redux State** - State shapes and actions
- **Streaming Protocol** - TCP message format
- **YouTube API** - Integration patterns
- **Database API** - SQLite operations via Microsoft.Data.Sqlite

**See [API.md](./API.md) for full API reference.**

---

# Current Repository State

## Project Structure

```
.
├── MusicPlayer/                    # Core library project
│   ├── MusicPlayer.csproj        # net10.0
│   ├── Controller/               # Business logic
│   ├── Models/                  # Data models
│   └── Interface/               # Contracts
├── MusicPlayerWeb/               # Avalonia UI project
│   ├── MusicPlayerWeb.csproj   # net10.0 + Avalonia + CefGlue
│   ├── MainWindow.axaml         # Avalonia XAML
│   ├── MusicPlayerGate.*.cs     # C# ↔ JS bridge
│   └── Web/                    # React frontend
│       ├── Pages/
│       ├── Scripts/
│       ├── Style/
│       └── Resources/
├── docs/                         # Documentation
│   ├── tasks.json              # Task list
│   ├── API.md
│   ├── ARCHITECTURE.md
│   ├── HOW-IT-WORKS.md
│   ├── TECHNOLOGY-STACK.md
│   └── INDEX.md                # This file
└── README.md
```

## Status

| Aspect | Status | Notes |
|--------|--------|-------|
| Functionality | ✅ Operational | All features working |
| Code State | ✅ Migrated | .NET 10 + Avalonia + CefGlue |
| Documentation | ✅ In Progress | Current state documented |
| Build System | ✅ .NET 10 | Cross-platform (win-x64, linux-x64, osx-x64) |
| Platform Support | ✅ Cross-Platform | Windows, Linux (tested), macOS (pending) |

## Known Limitations

1. **React Version**: May be outdated, consider upgrade
2. **.NET 10**: Recently migrated from .NET Framework 4.5.2
3. **Protocol**: Custom TCP (consider HTTP/WebSocket alternatives for future)
4. **macOS Testing**: Build works, needs runtime testing

## Upgrade History

- **2026-05-06**: Migrated to .NET 10 + Avalonia + CefGlue.Avalonia
- **Earlier**: .NET Framework 4.5.2 + WPF + CefSharp.Wpf

---

# Contributing

## When Making Changes

When you modify the codebase, update this documentation:

- **Architecture changed** → [ARCHITECTURE.md](./ARCHITECTURE.md)
- **New feature added** → [FEATURES.md](./FEATURES.md)
- **Library dependency added** → [TECHNOLOGY-STACK.md](./TECHNOLOGY-STACK.md)
- **New module/flow documented** → [HOW-IT-WORKS.md](./HOW-IT-WORKS.md)
- **New API endpoint/function** → [API.md](./API.md)
- **New doc added** → Update this INDEX.md

## Build Instructions

### Build Solution (.NET 10)

```bash
dotnet restore
dotnet build MusicPlayerWeb/MusicPlayerWeb.csproj
```

### Build for Specific Platform

```bash
# Windows
dotnet build MusicPlayerWeb/MusicPlayerWeb.csproj -r win-x64

# Linux
dotnet build MusicPlayerWeb/MusicPlayerWeb.csproj -r linux-x64

# macOS
dotnet build MusicPlayerWeb/MusicPlayerWeb.csproj -r osx-x64
```

### Build Frontend (React)

```bash
cd MusicPlayerWeb/Web
npm install
npm run webpack
```

### Run

```bash
dotnet run --project MusicPlayerWeb/MusicPlayerWeb.csproj
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
4. Cross-platform changes must be documented in TECHNOLOGY-STACK.md

---

# Version History

| Version | Date | Notes |
|--------|------|-------|
| .NET 10 + Avalonia | 2026-05-06 | Cross-platform support added |
| .NET Framework 4.5.2 + WPF | Prior | Windows-only, legacy |

---

# License & Attribution

- **Music Player**: See repository LICENSE
- **React**: MIT
- **Avalonia**: MIT
- **CefGlue**: See CefGlue license
- **NAudio**: BSD
- **TagLib#**: LGPL
- **YouTube**: Subject to YouTube Terms of Service

---

# Quick Reference

## Most Common Tasks

| Task | How-To | See |
|------|--------|-----|
| Import music | Click "Open Folder" | [Features](./FEATURES.md) |
| Host server | `/server` → Set port → "Host" | [Features](./FEATURES.md) |
| Connect client | `/client` → Enter IP:port → "Connect" | [Features](./FEATURES.md) |
| Play YouTube | `/video` → Paste URL → "Play" | [Features](./FEATURES.md) |
| Browse radio | `/radio` → Search → Click station | [Features](./FEATURES.md) |
| Copy files | `/copy` → Select folders → "Copy" | [Features](./FEATURES.md) |

## Cross-Platform Notes

- **Windows**: ✅ Tested, CEF 120 binaries included
- **Linux**: ✅ Tested (current build platform)
- **macOS**: ⚠️ Build verified, runtime testing pending

**See [API.md - Cross-Platform Notes](./API.md#7-cross-platform-notes) for details.**

---

# Support & Troubleshooting

**Common issues:**

- Videos not playing → Check YouTube API/iframe
- Can't connect → Verify IP/port/firewall
- Slow import → Large libraries may need batching
- Audio not working → Check NAudio output device

**See [HOW-IT-WORKS.md](./HOW-IT-WORKS.md) for troubleshooting.**

---

**End of documentation overview.**

For detailed information on any aspect, see the relevant documentation files above.
