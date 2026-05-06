# API Reference

This document describes the data structures, functions, and communication interfaces used by the Music Player application.

**Note:** This application uses a custom protocol rather than a traditional REST API. However, the data interfaces follow similar patterns.

---

## Table of Contents

1. [C Sharp Bridge API](#1-c-sharp-bridge-api)
2. [Redux State API](#2-redux-state-api)
3. [Streaming Protocol](#3-streaming-protocol)
4. [YouTube Integration API](#4-youtube-integration-api)
5. [Web Interface API](#5-web-interface-api)
6. [Database API](#6-database-api)

---

## 1. C Sharp Bridge API

The C# backend exposes methods to the JavaScript frontend via the `window.MusicPlayer` global object.

### Global Object: `window.MusicPlayer`

#### Methods

| Method | Signature | Description | Returns |
|------|------|------|------|
| `openFolder()` | `() => Promise<string>` | Opens a folder browser dialog to select music library | File path or null |
| `openFiles()` | `() => Promise<string[]>` | Opens file browser for multiple file selection | Array of file paths |
| `hostServer(port)` | `(port: number) => Promise<void>` | Starts hosting music on specified port | void |
| `disconnectServer()` | `() => Promise<void>` | Stops the music server | void |
| `connectToServer(ip, port)` | `(ip: string, port: number) => Promise<void>` | Connects to a remote server | void |
| `getVideoInfoFromPlaylist(playlistId)` | `(id: string) => Promise<string>` | Fetches playlist video metadata | JSON string |
| `getChannelVideos()` | `() => Promise<string>` | Fetches recent videos from channel | JSON string |
| `getStations(searchText)` | `(text: string) => Promise<string>` | Fetches radio stations | JSON string |
| `getDefaultIP()` | `() => Promise<string>` | Gets default local IP address | IP string |
| `getVolume()` | `() => Promise<number>` | Gets current volume level | 0-100 |
| `setVolume(value)` | `(value: number) => Promise<void>` | Sets global volume | void |
| `seekVideo(position)` | `(position: number) => Promise<void>` | Seeks to video position | void |
| `startVideo(url)` | `(url: string) => Promise<void>` | Starts YouTube video playback | void |
| `stopVideo()` | `() => Promise<void>` | Stops video playback | void |
| `copySongs(source, dest, count)` | `(src, dst, n: number) => Promise<void>` | Copies random songs | void |
| `selectFolder()` | `() => Promise<string>` | Opens folder picker for copy | File path |
| `seekVideo(position)` | `(position: number) => Promise<void>` | Seeks to position in video | void |

#### Global Functions (via window.CSSharpDispatcher)

| Function | Signature | Description |
|------|------|------|
| `dispatchSetCurrentSong(jsonSong)` | `(song: object) => void` | Sets current song in Redux |
| `dispatchSetServerinfo(jsonInfo)` | `(info: object) => void` | Updates server info in Redux |
| `dispatchSetCopyProgress(progress)` | `(percent: number) => void` | Updates copy progress |

---

## 2. Redux State API

The Redux store manages application state across all components.

### Store Structure

```javascript
// Reducer imports
import { currentSong } from './Reducers/CurrentSong.jsx';
import { serverInfo } from './Reducers/ServerInfo.jsx';
import { copyProgress } from './Reducers/Copy.jsx';

// State shape
const initialState = {
    currentSong: null,
    serverInfo: null,
    copyProgress: null
};

const store = createStore(
    combineReducers({
        currentSong,
        serverInfo,
        copyProgress
    }),
    initialState
);
```

### CurrentSong Reducer

**State slice:**
```javascript
{
    currentSong: null | SongObject
}
```

**SongObject structure:**
```javascript
{
    Title: string;
    Artist: string;
    Album: string;
    Genre: string;
    Year: string;
    Composer: string;
    TrackNumber: number;
    DiscNumber: number;
    BitRate: number;     // kbps
    SampleRate: number;  // Hz
    Channels: number;    // 2 or 6
    Duration: number;    // milliseconds
    FilePath: string;
    PlayCount: number;
    LastPlayed: string;  // ISO 8601
    // Additional TagLib# fields...
}
```

**Actions:**
```javascript
// Dispatch actions
window.CSSharpDispatcher.dispatchSetCurrentSong(songJson);

// In React
import { SongActions } from './DataStore/StoreActions';
store.dispatch(SongActions.setCurrentSong(song));
```

### ServerInfo Reducer

**State slice:**
```javascript
{
    serverInfo: null | ServerInfoObject
}
```

**ServerInfoObject structure:**
```javascript
{
    IsHost: boolean;           // true = hosting server
    Host: string;              // IP address
    Port: number;              // Port number
    Clients: Record<string, number>; // "{IP: port}" map
    VideoUrl: string | null;   // Current YouTube URL
    VideoPosition: number;     // Current position in ms
}
```

**Client info example:**
```javascript
{
    IsHost: false,
    Host: "192.168.1.100",
    Port: 8963,
    Clients: {
        "192.168.1.101": 8963,
        "192.168.1.102": 8963,
        "192.168.1.103": 8963
    },
    VideoUrl: "https://www.youtube.com/watch?v=abc123",
    VideoPosition: 123456
}
```

### CopyProgress Reducer

**State slice:**
```javascript
{
    copyProgress: null | number   // 0-100 percentage
}
```

**Actions:**
```javascript
store.dispatch(SongActions.changeCopyProgress(50)); // 50%
store.dispatch(SongActions.changeCopyProgress(100)); // Complete
```

---

## 3. Streaming Protocol

The custom TCP-based streaming protocol used between server and clients.

### Connection Lifecycle

```mermaid
sequenceDiagram
    participant ClientApp as Client Application
    participant TcpSocket as TCP Socket
    participant ServerApp as Server Application

    ClientApp->>TcpSocket: socket.connect(ip, port)
    TcpSocket-->>ClientApp: Connection established
    ClientApp->>TcpSocket: socket.write("CONNECT")
    TcpSocket->>TcpSocket: Send to server
    TcpSocket->>ServerApp: Receive "ACCEPTED" + audio stream
    TcpSocket->>ClientApp: Read audio data
    loop Audio Data
        TcpSocket->>TcpSocket: Read audio chunks
        TcpSocket->>ClientApp: Process audio frames
    end
```

### Protocol Format

**Connection handshake:**

```
Client -> Server: "CONNECT\r\n"
Server -> Client: "ACCEPTED\r\n"

// Then audio stream follows
```

**Audio frames:**
- Format: PCM audio data (or compressed)
- Size: 16KB chunks (efficient streaming)
- Timing: Real-time, no buffering (live)

### Server Commands

**Server can send to all clients:**

| Command | Format | Description |
|------|------|------|
| CONNECT | "CONNECT\r\n" | Allow new connection |
| PLAY | "PLAY:{volume}\r\n" | Start playing at volume |
| PAUSE | "PAUSE\r\n" | Pause playback |
| SEEK:{position} | "SEEK:123456\r\n" | Jump to position |
| VOLUME:{value} | "VOLUME:75\r\n" | Set volume |
| VIDEO:{url} | "VIDEO:https://youtube.com/..." | Switch video |
| POSITION:{position} | "POSITION:123456\r\n" | Current video position |

### Client Commands

**Client can send to server:**

| Command | Format | Description |
|------|------|------|
| CONNECT | "CONNECT\r\n" | Request connection |
| DISCONNECT | "DISCONNECT\r\n" | Close connection |
| PLAY | "PLAY:123456,85\r\n" | Play at position 123456 with volume 85 |
| PAUSE | "PAUSE\r\n" | Pause playback |
| POSITION_UPDATE:{position} | "POSITION_UPDATE:123456\r\n" | Sync position |
| VOLUME:{value} | "VOLUME:75\r\n" | Set volume |

---

## 4. YouTube Integration API

YouTube API endpoints used by the application.

### Iframe API

```javascript
// Create player
const player = new YT.Player('youtube-player', {
    height: '360',
    width: '640',
    videoId: 'VIDEOID',
    events: {
        'onReady': onReadyFunc,
        'onStateChange': onStateChangeFunc
    }
});

// Methods accessed via 'player' instance
player.loadVideoById(videoId);
player.playVideo();
player.pauseVideo();
player.stopVideo();
player.seekTo(time, allowSeekAhead);
player.setVolume(volume);
player.isPlaying() => boolean;
player.getCurrentTime() => number;
```

### URL Parsing API

```javascript
function extractVideoId(url) {
    const youtuBeRegex = /(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\- ]+)/;
    const match = url.match(youtuBeRegex);
    return match ? match[1] : null;
}

function extractPlaylistId(url) {
    const listRegex = /(list=)([^&]+)/;
    const match = url.match(listRegex);
    return match ? match[2] : null;
}

function extractFromWatchPage(url) {
    const watchRegex = /v=([a-zA-Z0-9_-]+)/;
    const match = url.match(watchRegex);
    return match ? match[1] : null;
}
```

### Playlist Data Structure

Response from YouTube Data API or scraped endpoints:

```javascript
[
    {
        ID: string;          // e.g., "abc123"
        Title: string;
        ThumbnailUrl: string; // e.g., "https://img.youtube.com/vi/abc123/mqdefault.jpg"
        Description: string;
        Duration: number;    // Optional
        Url: string;         // Direct video URL
        Views?: number;      // Optional
    }
]
```

---

## 5. Web Interface API

React web interface components API.

### Component Props

**Home Props:**
```javascript
{
    match: {
        isExact: boolean  // true if current route
    }
}
```

**Server Props:**
```javascript
{
    serverInfo: ServerInfoObject   // {IsHost, Host, Port, Clients, VideoUrl, VideoPosition}
}
```

**Client Props:**
```javascript
{
    serverInfo: ServerInfoObject
}
```

**Video Props:**
```javascript
{
    serverInfo: ServerInfoObject
}
```

### Event Handlers

```javascript
// Home component
openFolder()          // Open folder dialog
openFiles()           // Open file dialog
gotoServer()          // Navigate to /server
gotoClient()          // Navigate to /client
gotoCopy()            // Navigate to /copy
goto(url)             // Navigate to route

// Server component
hostServer(port: number)    // Host server
disconnectServer()          // Disconnect all
```

---

## 6. Database API

SQLite database operations via System.Data.SQLite.LINQ.

### Db Class Interface

```csharp
public static class Db
{
    // Get entities
    public static IEnumerable<Song> GetSongs() => songTable.ToArray();
    public static IEnumerable<Album> GetAlbums() => albumTable.ToArray();
    public static IEnumerable<Artist> GetArtists() => artistTable.ToArray();
    public static IEnumerable<Playlist> GetPlaylists() => playlistTable.ToArray();
    
    // Search
    public static IEnumerable<Song> SearchSongs(string query) => ...
    public static IEnumerable<Song> SearchByArtist(string artist) => ...
    public static IEnumerable<Song> SearchByAlbum(string album) => ...
    
    // Get by ID
    public static Song GetSongById(int id) => ...
    public static Song GetSongByPath(string path) => ...
    
    // Add
    public static void AddSong(Song song) => ...
    public static void AddAlbum(Album album) => ...
    
    // Update
    public static void UpdateSong(Song song) => ...
    public static void DeleteSong(Song song) => ...
    
    // Stats
    public static int GetTotalSongs() => ...
    public static long GetTotalSize() => ...
    
    // Path management
    public static void ScanFolder(string folderPath) => ...
    public static void ScanFiles(string[] filePaths) => ...
    public static void DeleteMissingFiles(string[] missingPaths) => ...
}
```

### Entity Tables

**Songs Table:**
```sql
CREATE TABLE Songs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT,
    Artist TEXT,
    Album TEXT,
    Genre TEXT,
    TrackNumber INTEGER,
    DiscNumber INTEGER,
    Year TEXT,
    BitRate INTEGER,
    SampleRate INTEGER,
    Channels INTEGER,
    Duration INTEGER,  -- milliseconds
    FilePath TEXT,
    PlayCount INTEGER DEFAULT 0,
    LastPlayed TEXT,
    Picture TEXT,  -- Base64 or file path
    FOREIGN KEY (Artist) REFERENCES Artists(Id),
    FOREIGN KEY (Album) REFERENCES Albums(Id)
);
```

**Albums Table:**
```sql
CREATE TABLE Albums (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Artist TEXT,  -- Artist ID or name
    Year TEXT,
    Genre TEXT,
    Picture TEXT
);
```

**Artists Table:**
```sql
CREATE TABLE Artists (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT UNIQUE NOT NULL
);
```

**Playlists Table:**
```sql
CREATE TABLE Playlists (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Creator TEXT,
    Picture TEXT
);
```

**Playlist-Song Junction:**
```sql
CREATE TABLE PlaylistSongs (
    PlaylistId INTEGER,
    SongId INTEGER,
    Position INTEGER,
    PRIMARY KEY (PlaylistId, SongId),
    FOREIGN KEY (PlaylistId) REFERENCES Playlists(Id),
    FOREIGN KEY (SongId) REFERENCES Songs(Id)
);
```

### Usage Examples

```csharp
// Load all songs
var songs = Db.GetSongs().ToList();

// Search
var matching = Db.SearchSongs("rock").ToList();

// Get file path
var song = Db.GetSongByPath(fullPath);

// Increment play count
Db.UpdateSong(song);
song.PlayCount++;
Db.SaveChanges();

// Scan folder
newFolder = "C:\\Music\\NewArtist";
Db.ScanFolder(newFolder);
```

---

## 7. Network Configuration

### Port Configuration

The application uses a configurable port for streaming:

**Default:** 8963

**Changing port:**
```csharp
// In Server.jsx
this.state = { port: 8963 };  // User can change this

// In C#
MusicPlayer.hostServer(port);
```

### Firewall Requirements

For remote access:
1. Port must be forwarded in router
2. Windows Firewall rule added
3. No other applications using port

### Security Recommendations

- Use port forwarding for external access
- Consider HTTPS/TLS for future upgrade
- Implement authentication layer
- Validate incoming protocol commands
- Monitor for malformed requests

---

## Appendix: Error Handling

### JavaScript Errors

```javascript
// Try-catch for async operations
try {
    await MusicPlayer.openConnection();
} catch (e) {
    console.error("Connection failed", e);
}

// Handle network errors
window.addEventListener('error', (e) => {
    if (e.message.includes('Socket')) {
        // Reconnect attempt
    }
});
```

### C# Errors

```csharp
try {
    hostServer(port);
} catch (InvalidOperationException) {
    // Invalid port
} catch (SocketException) {
    // Socket issue
} catch (Exception ex) {
    // Log and handle
    Logger.LogError(ex);
}
```

---

**End of API Reference documentation**
