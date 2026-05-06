---
name: music-player-task-planning
description: Task planning guidelines for Music Player Maintainer agent, including task JSON structure and step examples.
category: music-player
---

# Music Player Task Planning

## Task JSON Format

Use this structure for `docs/tasks.json`:

```json
{
  "meta": {
    "agent_name": "Documentation-Maintainer",
    "storage_file": "docs/tasks.json"
  },
  "tasks": [
    {
      "id": "unique-identifier",
      "title": "Short summary of task",
      "status": "pending|in_progress|complete",
      "subtasks": [
        {
          "step": 1,
          "action": "File operation or command",
          "description": "Detailed description of what to do",
          "file": "relative/path/to/file",
          "operation": "create|update|delete|verify",
          "content_hint": "Notes about content, examples, etc.",
          "status": true|false
        }
      ]
    }
  ]
}
```

## Step Examples

### Documentation Update Step
```json
{
  "step": 5,
  "action": "Update ARCHITECTURE.md",
  "description": "Add new WebSocket server component",
  "file": "docs/architecture.md",
  "operation": "update",
  "content_hint": "Add component in server streaming architecture section"
}
```

### Code Implementation Step
```json
{
  "step": 3,
  "action": "Implement Spotify connector service",
  "description": "Create SpotifyAPI class with OAuth 2.0",
  "file": "MusicPlayer/SpotifyAPI.cs",
  "operation": "create",
  "content_hint": "Include OAuth flow, token refresh, song fetching methods"
}
```

### Feature Documentation Step
```json
{
  "step": 7,
  "action": "Add Spotify feature to FEATURES.md",
  "description": "Document Spotify integration feature",
  "file": "docs/features.md",
  "operation": "update",
  "content_hint": "Add new section under 'Music Source Integration'"
}
```