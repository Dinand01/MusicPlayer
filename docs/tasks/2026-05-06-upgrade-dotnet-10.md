# Task: Upgrade to .NET 10

**Task ID:** `2026-05-06-upgrade-dotnet-10`
**Date:** `2026-05-06`
**Status:** `pending`
**Linked JSON Task:** `docs/tasks.json` → task ID `2026-05-06-upgrade-dotnet-10`

---

## Overview

Upgrade the Music Player C# application from .NET Framework 4.5.2 to .NET 10. This involves converting legacy .csproj files to SDK-style format, replacing incompatible components (WCF → gRPC/Websockets, CefSharp 63 → latest), and updating all NuGet packages. The UI wrapper is WPF (not Windows Forms).

---

## Acceptance Criteria

- [ ] MusicPlayer.csproj and MusicPlayerWeb.csproj converted to SDK-style with net10.0 target
- [ ] WCF services replaced with gRPC (preferred) or Websockets
- [ ] CefSharp.Wpf updated to .NET 10 compatible version
- [ ] EntityFramework 6 migrated to EF Core 10
- [ ] System.Data.SQLite replaced with Microsoft.Data.Sqlite
- [ ] All NuGet packages updated to .NET 10 compatible versions
- [ ] WPF UI loads and functions correctly
- [ ] All documentation updated (TECHNOLOGY-STACK.md, ARCHITECTURE.md, HOW-IT-WORKS.md, API.md, INDEX.md)

---

## Detailed Instructions

### Step 0: Solution Discovery
**File(s):** `MusicPlayer/Controller/WCFServerClient.cs`, `MusicPlayer/Controller/WCFServerService.cs`, `MusicPlayer/Interface/IClientContract.cs`, `MusicPlayer/Interface/IServerContract.cs`
**Operation:** `verify`
**Status:** `[ ] Pending`

**What to do:**
Perform research, analysis, and sanity-check of the proposed solution before planning implementation steps.

**Discovery checklist:**
- [ ] Review WCF contracts (IClientContract, IServerContract) to understand service surface
- [ ] Identify all files affected by WCF removal
- [ ] Evaluate gRPC vs Websockets for WCF replacement (check proto definition feasibility)
- [ ] Verify CefSharp.Wpf latest version supports .NET 10
- [ ] Check EF Core 10 migration path from EF6
- [ ] Confirm Microsoft.Data.Sqlite is drop-in replacement for System.Data.SQLite

**Discovery summary:**
[To be filled after research - will document component replacement matrix and migration approach]

---

### Step 1: Convert MusicPlayer.csproj to SDK-style format
**File(s):** `MusicPlayer/MusicPlayer.csproj`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Convert legacy VS2015-style .csproj to SDK-style format for .NET 10 compatibility. Remove all explicit assembly references and NuGet package references (will be handled by PackageReference format).

**Expected changes:**
- [ ] Remove ToolsVersion, Import, and explicit Reference elements
- [ ] Add SDK-style project declaration
- [ ] Convert to PackageReference format for NuGet packages
- [ ] Set TargetFramework to net10.0

**Verification:**
- [ ] Project loads without errors in IDE
- [ ] `dotnet build MusicPlayer/MusicPlayer.csproj` succeeds

---

### Step 2: Convert MusicPlayerWeb.csproj (WPF) to SDK-style format
**File(s):** `MusicPlayerWeb/MusicPlayerWeb.csproj`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Convert WPF project to SDK-style format. Preserve WPF-specific properties (OutputType=WinExe, ProjectTypeGuids for WPF).

**Expected changes:**
- [ ] Remove legacy project format elements
- [ ] Add SDK-style project with WPF support
- [ ] Convert to PackageReference format
- [ ] Set TargetFramework to net10.0
- [ ] Preserve XAML compilation settings

**Verification:**
- [ ] Project loads without errors
- [ ] `dotnet build MusicPlayerWeb/MusicPlayerWeb.csproj` succeeds

---

### Step 3: Update target framework to net10.0 in both projects
**File(s):** `MusicPlayer/MusicPlayer.csproj`, `MusicPlayerWeb/MusicPlayerWeb.csproj`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Ensure both projects target .NET 10 explicitly.

**Expected changes:**
- [ ] Set `<TargetFramework>net10.0</TargetFramework>` in both projects

**Verification:**
- [ ] Both projects restore and build for net10.0

---

### Step 4: Research gRPC compatibility with current WCF contract
**File(s):** `MusicPlayer/Interface/IClientContract.cs`, `MusicPlayer/Interface/IServerContract.cs`
**Operation:** `verify`
**Status:** `[ ] Pending`

**What to do:**
Analyze WCF contracts and design gRPC proto files that match the service surface. If gRPC is not feasible, document Websocket message protocol.

**Expected changes:**
- [ ] Create `MusicPlayer/Protos/musicplayer.proto` (if gRPC) OR `MusicPlayer/Models/WebSocketMessages.cs` (if websockets)
- [ ] Document decision: gRPC vs Websockets

**Verification:**
- [ ] Proto file compiles OR websocket protocol documented

---

### Step 5: Implement gRPC services (preferred) OR Websocket transport layer
**File(s):** `MusicPlayer/Controller/`, `MusicPlayer/Protos/` or `MusicPlayer/Controllers/WebSocket/`
**Operation:** `create`
**Status:** `[ ] Pending`

**What to do:**
Implement new transport layer. For gRPC: create proto files, implement server/client. For Websockets: implement message-based communication.

**Expected changes:**
- [ ] Create proto files and generate C# code (gRPC path)
- [ ] OR implement WebSocket server/client classes
- [ ] Add Grpc.AspNetCore or WebSocket packages

**Verification:**
- [ ] New transport layer compiles and basic ping works

---

### Step 6: Refactor WCFServerClient.cs → gRPC client / Websocket client
**File(s):** `MusicPlayer/Controller/WCFServerClient.cs`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Replace WCF client implementation with gRPC client or Websocket client that implements IClientContract interface.

**Expected changes:**
- [ ] Remove WCF-specific code
- [ ] Implement new client using gRPC/Websockets
- [ ] Maintain IClientContract interface compatibility

**Verification:**
- [ ] Client connects to server using new transport

---

### Step 7: Refactor WCFServerService.cs → gRPC server / Websocket server
**File(s):** `MusicPlayer/Controller/WCFServerService.cs`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Replace WCF service implementation with gRPC service or Websocket server implementing IServerContract interface.

**Expected changes:**
- [ ] Remove WCF ServiceHost code
- [ ] Implement new server using gRPC/Websockets
- [ ] Maintain IServerContract interface compatibility

**Verification:**
- [ ] Server starts and accepts connections

---

### Step 8: Update interfaces for new transport
**File(s):** `MusicPlayer/Interface/IClientContract.cs`, `MusicPlayer/Interface/IServerContract.cs`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Update interface definitions if needed to match gRPC proto or Websocket message structure.

**Expected changes:**
- [ ] Adjust interfaces for async/await patterns if needed
- [ ] Ensure compatibility with new transport

**Verification:**
- [ ] Interfaces compile with new implementation

---

### Step 9: Upgrade CefSharp.Wpf 63 → latest CefSharp.Wpf
**File(s):** `MusicPlayerWeb/MusicPlayerWeb.csproj`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Update CefSharp.Wpf from version 63.0.3 to latest version supporting .NET 10.

**Expected changes:**
- [ ] Remove old CefSharp 63 NuGet packages
- [ ] Add latest CefSharp.Wpf NuGet package
- [ ] Update CefSharp initialization code if API changed

**Verification:**
- [ ] CefSharp loads correctly in WPF app
- [ ] Web content displays properly

---

### Step 10: Migrate EntityFramework 6 → EF Core 10
**File(s):** `MusicPlayer/Db.cs`, `MusicPlayer/Models/`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Migrate from EntityFramework 6.1.3 to EF Core 10. Update DbContext and model configurations.

**Expected changes:**
- [ ] Remove EF6 packages, add EF Core 10
- [ ] Update Db.cs to use EF Core patterns
- [ ] Update model classes for EF Core conventions
- [ ] Create migration for existing database schema

**Verification:**
- [ ] Database operations work with EF Core
- [ ] Existing database schema preserved

---

### Step 11: Replace System.Data.SQLite → Microsoft.Data.Sqlite
**File(s):** `MusicPlayer/MusicPlayer.csproj`, `MusicPlayer/Db.cs`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Replace System.Data.SQLite 1.0.108 with Microsoft.Data.Sqlite (part of EF Core or standalone).

**Expected changes:**
- [ ] Remove System.Data.SQLite NuGet packages
- [ ] Add Microsoft.Data.Sqlite package
- [ ] Update connection string and provider configuration

**Verification:**
- [ ] SQLite database connects and queries execute

---

### Step 12: Update NAudio to .NET 10 compatible version
**File(s):** `MusicPlayer/MusicPlayer.csproj`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Update NAudio from 1.8.4 to latest version supporting .NET 10.

**Expected changes:**
- [ ] Update NAudio NuGet package
- [ ] Fix any API changes in audio playback code

**Verification:**
- [ ] Audio playback works correctly

---

### Step 13: Update all remaining NuGet packages
**File(s):** `MusicPlayer/MusicPlayer.csproj`, `MusicPlayerWeb/MusicPlayerWeb.csproj`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Update Newtonsoft.Json, NLog, AngleSharp, YoutubeExplode, TagLib, and any other packages to .NET 10 compatible versions.

**Expected changes:**
- [ ] Update all PackageReference versions
- [ ] Fix any breaking API changes

**Verification:**
- [ ] All packages restore successfully
- [ ] No deprecated package warnings

---

### Step 14: Fix code breaking changes for .NET 10
**File(s):** `MusicPlayer/`, `MusicPlayerWeb/`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Fix any code that breaks due to .NET 10 changes (removed APIs, namespace changes, etc.).

**Expected changes:**
- [ ] Replace obsolete APIs
- [ ] Update using statements
- [ ] Fix any compilation errors

**Verification:**
- [ ] Full solution builds without errors

---

### Step 15: Update MusicPlayerWrapper.cs (WPF-specific code)
**File(s):** `MusicPlayer/Controller/MusicPlayerWrapper.cs`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Ensure WPF wrapper works with .NET 10 and updated CefSharp.

**Expected changes:**
- [ ] Update WPF interop code if needed
- [ ] Verify CefSharp integration

**Verification:**
- [ ] WPF wrapper functions correctly

---

### Step 16: Test WPF UI loads correctly with CefSharp
**File(s):** `MusicPlayerWeb/`
**Operation:** `verify`
**Status:** `[ ] Pending`

**What to do:**
Run the WPF application and verify all UI components load and function.

**Expected changes:**
- [ ] Main window displays
- [ ] CefSharp browser works
- [ ] All features accessible

**Verification:**
- [ ] Full UI test pass

---

### Step 17: Update TECHNOLOGY-STACK.md
**File(s):** `docs/technology-stack.md`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Document new .NET 10 framework, gRPC/Websockets, updated packages.

**Expected changes:**
- [ ] Update .NET Framework → .NET 10
- [ ] Add gRPC or Websockets to tech stack
- [ ] List all updated packages with versions

**Verification:**
- [ ] Tech stack reflects current state

---

### Step 18: Update ARCHITECTURE.md
**File(s):** `docs/architecture.md`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Update architecture diagram to show WCF → gRPC/Websockets transition.

**Expected changes:**
- [ ] Update communication layer documentation
- [ ] Add new component diagrams

**Verification:**
- [ ] Architecture docs match implementation

---

### Step 19: Update HOW-IT-WORKS.md
**File(s):** `docs/how-it-works.md`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Document new communication flows (gRPC/Websockets) and updated components.

**Expected changes:**
- [ ] Update WCF → gRPC/Websockets flow diagrams
- [ ] Document new startup sequence

**Verification:**
- [ ] How-it-works reflects current implementation

---

### Step 20: Update API.md
**File(s):** `docs/api.md`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Document gRPC proto definitions OR Websocket message specifications.

**Expected changes:**
- [ ] Add gRPC service/method documentation OR
- [ ] Add Websocket message protocol documentation

**Verification:**
- [ ] API docs match implementation

---

### Step 21: Update INDEX.md
**File(s):** `docs/index.md`
**Operation:** `update`
**Status:** `[ ] Pending`

**What to do:**
Update table of contents with any new documentation links.

**Expected changes:**
- [ ] Verify all doc links are current
- [ ] Add any new sections

**Verification:**
- [ ] INDEX.md TOC is accurate

---

### Step 22: Verify all docs are current (final step)
**File(s):** `docs/`
**Operation:** `verify`
**Status:** `[ ] Pending`

**What to do:**
Final verification that all documentation matches the upgraded implementation.

**Expected changes:**
- [ ] Review all updated docs for accuracy
- [ ] Ensure no stale references to .NET 4.5.2 or WCF

**Verification:**
- [ ] All documentation is current and accurate
- [ ] Task is complete

---

## Documentation Updates Required

- [ ] `docs/INDEX.md` - Update table of contents
- [ ] `docs/ARCHITECTURE.md` - WCF → gRPC/Websockets architecture
- [ ] `docs/TECHNOLOGY-STACK.md` - .NET 10, new packages, gRPC/Websockets
- [ ] `docs/HOW-IT-WORKS.md` - New communication flow
- [ ] `docs/API.md` - gRPC proto OR websocket message specs

---

## Notes / Context

- UI Framework: WPF (confirmed via ProjectTypeGuids and CefSharp.Wpf usage)
- WCF Replacement Decision: gRPC preferred, Websockets as fallback
- Legacy project format requires full SDK-style conversion
- CefSharp 63 is very old (2018) - major update needed
- User constraint: Pure REST is not an option (need persistent connection like gRPC/Websockets)

---

## Review & Approval

**Planned by:** `MusicPlayer-Maintainer`
**Date:** `2026-05-06`
**Approved by:** `[Pending]`
**Approval date:** `[Pending]`

---

**Template version:** 1.0
**Location:** `docs/tasks/.template.md`
