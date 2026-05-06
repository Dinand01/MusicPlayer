# Task: Upgrade to .NET 10

**Date:** 2026-05-06  
**Status:** In Progress  
**Agent:** MusicPlayer-Maintainer

---

## Overview

Migrate Music Player from .NET Framework 4.5.2 to .NET 10, replacing deprecated WCF communication with modern alternatives (gRPC or Websockets), upgrading all NuGet packages, and updating documentation.

---

## Step 0: Solution Discovery - WCF Analysis

### Current WCF Implementation

**Contracts:**
- `IServerContract` - Duplex service contract with callback to `IClientContract`
- `IClientContract` - Callback contract (all methods are OneWay/fire-and-forget)

**Methods to Migrate:**

| Contract | Method | Signature | Notes |
|----------|--------|-----------|-------|
| IServerContract | Anounce | void Anounce() | Client connection notification |
| IServerContract | Goodbye | void Goodbye() | Client disconnection notification |
| IServerContract | GetCurrentPosition | double? GetCurrentPosition() | **Only non-OneWay method** - returns data |
| IClientContract | PlayVideo | void PlayVideo(string video) | OneWay |
| IClientContract | SeekVideo | void SeekVideo(double position) | OneWay |
| IClientContract | SetSongPosition | void SetSongPosition(double position) | OneWay |
| IClientContract | SetSong | void SetSong(SongInformation song) | OneWay, complex object |
| IClientContract | SendFile | void SendFile(Stream stream) | **OneWay with Stream** - challenging for gRPC |
| IClientContract | Play | void Play() | OneWay |
| IClientContract | PlayRadio | void PlayRadio(SongInformation radioInfo, string url) | OneWay, complex object |
| IClientContract | Pause | void Pause() | OneWay |
| IClientContract | Disconnect | void Disconnect() | OneWay |

### Replacement Options Analysis

#### Option A: gRPC (Recommended)

**Pros:**
- Native .NET Core/10 support
- Strongly typed contracts via proto files
- Excellent performance
- Built-in code generation
- Streaming support (bidirectional)

**Cons:**
- `SendFile(Stream)` needs adaptation (gRPC uses different streaming model)
- Need to define proto messages for `SongInformation`
- Duplex pattern requires gRPC bidirectional streaming or separate client/server stubs

**Approach for gRPC:**
- Define `SongInformation` as proto message
- Use client streaming or unary calls for file transfer (chunk file into bytes)
- Duplex communication: server exposes methods, client calls them + server can call client via separate connection or callback pattern
- `GetCurrentPosition()` → Unary RPC (request/response)
- All OneWay methods → Unary RPCs (fire-and-forget pattern on client side)

#### Option B: Websockets

**Pros:**
- Simple, widely supported
- Natural bidirectional communication
- Easy to send arbitrary data (including file streams)

**Cons:**
- No built-in contract enforcement (need custom protocol)
- More manual serialization/deserialization
- No code generation

**Approach for Websockets:**
- Define JSON message protocol with action types
- Use System.Net.WebSockets or WebSocketSharp
- SendFile can stream directly
- Need to implement message routing/filtering

### Decision: gRPC (Preferred)

**Rationale:**
1. Better .NET 10 integration
2. Strong typing prevents runtime errors
3. Performance benefits
4. Industry standard for microservices

**Migration Strategy:**
1. Define .proto file with all messages and services
2. Generate C# code from proto
3. Implement gRPC server (replacing WCFServerService)
4. Implement gRPC client (replacing WCFServerClient)
5. Handle `SendFile` by chunking file into `bytes` field or using client streaming RPC
6. Maintain `IClientContract` and `IServerContract` interfaces if other code depends on them (adapter pattern)

### Component Compatibility Check

| Component | Current | .NET 10 Status | Action |
|-----------|---------|----------------|--------|
| CefSharp.Wpf | 63 | Needs upgrade to latest | Update to CefSharp.Wpf .NET 10 compatible version |
| EntityFramework | 6 | Not compatible | Migrate to EF Core 10 |
| System.Data.SQLite | - | Not .NET Core compatible | Replace with Microsoft.Data.Sqlite |
| NAudio | - | Check latest version | Update to .NET 10 compatible version |
| Newtonsoft.Json | - | Compatible but consider System.Text.Json | Update to latest |
| NLog | - | Compatible | Update to latest |
| AngleSharp | - | Compatible | Update to latest |
| YoutubeExplode | - | Compatible | Update to latest |
| TagLib | - | Check compatibility | Update to latest |

---

## Next Steps

Proceed to Step 1: Convert MusicPlayer.csproj to SDK-style format
