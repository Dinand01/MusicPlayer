# Features

## Core Features

### 1. Music Library Management

#### 1.1 Folder Import
- Browse and select folders containing audio files
- Scans recursively and loads all supported audio formats
- Handles large libraries efficiently using NAudio's optimized parsing

#### 1.2 File Import  
- Select multiple individual files at once
- Supports common formats (mp3, wav, flac, ogg, etc.)
- Uses TagLib# for metadata extraction

#### 1.3 Metadata Display
Shows for each track:
- Title, Artist, Album, Genre
- Track number, Disc number, Composer
- Play count, last played date
- Bitrate, sample rate, duration
- Additional TagLib# supported metadata

### 2. Music Streaming

#### 2.1 Server Mode (`/server`)
- Host a local music server
- Configure port (default: 8963)
- Broadcast to multiple clients simultaneously
- View list of connected clients
- Disconnect clients manually

#### 2.2 Client Mode (`/client`)
- Connect to a remote music server
- Enter IP address and port
- Access library hosted elsewhere
- Full playback controls
- Join public or private streams

#### 2.3 Streaming Architecture
- Uses custom TCP-based protocol
- Server pushes audio data to clients
- Efficient bandwidth usage
- Minimal latency
- Synchronized playback across clients

#### 2.4 YouTube Synchronization
- Host broadcasts YouTube videos
- All clients synchronized to same video
- Position tracking maintained
- Volume synchronization
- Playlist support for continuous playback

### 3. YouTube Video Streaming (`/video`)

#### 3.1 Single Videos
- Paste YouTube URL
- Auto-converts various URL formats
- Plays video in embedded player
- Full YouTube controls retained

#### 3.2 Playlists
- Detects `list=` parameter
- Fetches playlist metadata via YouTube API
- Displays video thumbnails and titles
- Click to select video
- Auto-next on completion
- Continues from last position on return to server

#### 3.3 Channel Videos
- Fetches recent videos from channel
- Browse available content
- Mark favorites
- Play in sync with server (when connected)

#### 3.4 Video State Tracking
Tracks and manages:
- Currently playing video
- Buffering state
- Playing/paused state
- Volume level
- End-of-video transition
- Server sync when connected

### 4. Internet Radio (`/radio`)

#### 4.1 Station Discovery
- Search text filter
- Paginated results (25 per page)
- Browse by genre, region, or keyword
- Load more stations as needed

#### 4.2 Station Management
- Add favorite stations
- Edit station entries
- Delete unwanted stations
- Persistent across sessions

#### 4.3 Playback Features
- Tune in to stream
- Stream metadata displayed
- Cover art if available
- Title and artist info

#### 4.4 Data Source
Uses:
- API key (Dirble in current implementation)
- Search and query endpoints
- Station metadata parsing

### 5. File Copy Utility (`/copy`)

#### 5.1 Folder Selection
- Select source folder
- Select destination folder
- Visual confirmation of paths
- Change selection at any time

#### 5.2 Copy Configuration
- Specify number of files to copy (default: 500)
- Configurable limits
- Progress tracking

#### 5.3 Progress Display
- Circular progress indicator
- Percentage displayed
- Real-time updates
- Visual feedback during copy

#### 5.4 Copy Logic
- Random file selection
- Avoids duplicates
- Respects file count limit
- Background copy operation
- User can continue using app during copy

### 6. User Interface Elements

#### 6.1 Navigation
- Fixed top navigation bar
- Dynamic menu items based on context
  - Home (always visible)
  - Playlist (when song/album playing)
  - Server (when hosting)
  - Client (when connected)
  - Video (when YouTube playing)
  - Radio (when stations available)
  - Copy (when file copy active)
- React Router for client-side routing
- Back/forward browser history

#### 6.2 Responsive Design
- CSS Grid/Flexbox layout
- Adaptive to window size
- Mobile-friendly controls
- Consistent navigation

#### 6.3 Component Structure

```
App.jsx (root)
├── Router (React Router)
│   ├── Home (main menu carousel)
│   ├── PlayList (song list + current track)
│   ├── Server (host server interface)
│   ├── Client (connect to server interface)
│   ├── Copy (file copy utility)
│   ├── Video (YouTube player + controls)
│   ├── Radio (station list + search)
│   └── EditRadio (station editor)
```

### 7. State Management

#### 7.1 Redux Store
Uses Redux for centralized state with:
- `store.dispatch()` for state updates
- `connect()` for component wiring
- Pure reducer functions
- Immutable state updates

#### 7.2 State Slices
- `currentSong` - Currently playing audio
- `serverInfo` - Server connection status
- `copyProgress` - File copy progress

#### 7.3 CSharpDispatcher
JavaScript bridge that:
- Exposes C# functions via global window object
- Maps Redux actions to C# calls
- Handles async operations
- Returns data to React

### 8. Build & Tooling

#### 8.1 Webpack Configuration
- Babel for JSX/ES6 transpilation
- SASS/SCSS for CSS preprocessing
- URL loader for assets < 30KB
- HTMLWebpackPlugin for entry point
- ExtractTextPlugin for CSS splitting

#### 8.2 npm Scripts
- `npm run webpack` - Production build
- `npm run start` - Watch mode for development

#### 8.3 React Versions (legacy)
- React 15.5.4 (Note: outdated)
- React Router 4.1.1 (legacy)
- Redux 3.6.0 (legacy)

### 9. System Integration

#### 9.1 Windows Integration (WPF)
- File dialogs (OpenFolder, OpenFiles)
- Windows Registry (installer)
- System tray integration (via MainWindow)
- Direct file system access

#### 9.2 Installer
- Creates shortcut
- Writes registry entries
- Installs dependencies
- Configures initial folder

#### 9.3 PowerShell Scripts
- `Initialize.ps1` - Quick setup script
- Can be run from command line
- Automated configuration

## Feature Limitations & Notes

### Current Limitations
1. **React Version**: Uses React 15.5.4 (very outdated)
   - Migrate to v16+ recommended
   
2. **YouTube API**: Uses iframe embed API
   - Not official YData API
   - Subject to YouTube policy changes
   
3. **Network Protocols**: Custom TCP implementation
   - Not HTTP-based
   - Requires port forwarding for remote access
   
4. **Browser**: CefSharp embedded
   - Windows only
   - Requires system DLLs

### Best Practices
- Use `Initialize.ps1` for setup
- Configure firewall for streaming port
- Use strong network connections for best streaming
- Store radio station lists locally
- Regular music library scans

### Platform Support
- **Windows**: Primary platform
- Requires .NET Framework 4.5.2
- No Linux/Mac support currently

### Security Considerations
- Streaming port exposure (firewall required)
- YouTube API key security
- No HTTPS enforcement currently
- Client authentication not implemented

## Future Enhancement Possibilities

### Recommended Upgrades
- Migrate React to v17+
- Use React 18 concurrent features
- Modernize Redux to 5.x
- Switch to modern CSS framework
- Implement HTTPS/TLS options
- Add authentication
- Cross-platform via .NET MAUI
- Use official YouTube Data API v3
- Implement proper streaming protocols (Icecast/Shoutcast)
- WebSocket for real-time updates

### Potential Features
- Spotify/Apple Music integration
- Podcast support
- Gapless playback optimization
- Sleep timer
- Cross-device sync
- Cloud storage for stations
- Dark mode toggle
- Custom skins/themes
- Playlist creation tools

## Feature Usage Guide

### Quick Start
1. Launch app
2. Click `/radio` to discover stations (first time)
3. Click `/home` → Open folder/files
4. Click `/server` to host music
5. Click `/video` for YouTube

### Streaming Setup
```
Server:
1. Configure IP (auto-detects)
2. Configure port (default 8963)
3. Click "Host"
4. Clients connect via IP:port

Client:
1. Enter server IP
2. Enter port (or use default)
3. Click "Connect"
```

### YouTube Workflow
```
Video Mode:
1. Paste YouTube URL
2. Click play (if auto-play fails)
3. Adjust volume
4. Navigate (auto-next on playlist)

Synced:
- When server has URL (host plays)
- Position syncs automatically
- Clients can't change video
- Host can stop any client
```

### File Copy Usage
```
1. Select source folder
2. Select destination folder
3. Enter file count
4. Click "Copy"
5. Monitor progress
6. Cancel anytime (if needed)
```

## Appendix: Feature Status Table

| Feature          | Status    | Notes                           |
|------------------|-----------|--------------------------------|
| Music Library    | ✅ Active | Large libraries supported      |
| Server Mode      | ✅ Active | TCP streaming                  |
| Client Mode      | ✅ Active | Connect to any server          |
| YouTube Videos   | ✅ Active | Embed player + API            |
| Playlists        | ✅ Active | Via YouTube playlist API      |
| YouTube Sync     | ✅ Active | Position/volume sync          |
| Internet Radio   | ✅ Active | Search + favorites           |
| File Copy        | ✅ Active | Random file copy              |
| Client List      | ✅ Active | Shows connected clients      |
| Metadata Display | ✅ Active | TagLib# extraction           |
