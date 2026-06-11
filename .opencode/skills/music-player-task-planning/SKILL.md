---
name: music-player-task-planning
description: Task planning guidelines for Music Player Maintainer agent, including task JSON structure and step examples.
---

# Music Player Task Planning

## Task Registry Format

Use `docs/TASKS.md` as the primary editable task registry. Tasks are listed newest-first so agents can read the beginning of the file to find the current priority task.

### Markdown Structure

```markdown
# Task Registry

**Meta:**
- **Agent Name:** MusicPlayer-Maintainer
- **Last Updated:** yyyy-MM-dd

## Active Tasks (N)

### Task N: [Task Title]

**ID:** `yyyy-MM-dd-description`  
**Status:** pending|in_progress|complete  
**Task File:** `docs/tasks/[task-id].md`

#### Steps:

| # | Action | File(s) | Status |
|---|--------|---------|--------|
| 0 | Solution Discovery | `path/to/file` | ✅/❌/⚠️ |
| 1 | Convert project to SDK format | `file.csproj` | ✅ |

---

## Status Symbols

Use these symbols in the markdown table Status column:
- ✅ - Completed
- ❌ - Not completed / Pending
- ⚠️ - Partial / In progress
- 🔄 - In progress

---

## Step Examples

### Documentation Update Step
```markdown
| 5 | Update ARCHITECTURE.md | `docs/architecture.md` | ❌ |
```

### Code Implementation Step
```markdown
| 3 | Convert MusicPlayer.csproj to SDK-style format | `MusicPlayer/MusicPlayer.csproj` | ❌ |
```

### Feature Documentation Step
```markdown
| 7 | Add Spotify feature to FEATURES.md | `docs/features.md` | ❌ |
```