---
name: music-player-docs-maintenance
description: Guidelines for maintaining documentation in the Music Player project, including update detection, file management, and verification rules.
category: music-player
---

# Music Player Documentation Maintenance

## Core Documentation Principles
1. **Documentation First**: Every code change requires documentation updates
2. **Verify Before Complete**: Last step of every task must verify documentation status
3. **Update TOC**: Always update `docs/index.md` with new links/changes after adding docs

## Documentation Update Detection

### Triggers (automatic):
- File created: Check if new feature needs doc
- Code changed: Check affected docs
- Library added: Check `TECHNOLOGY-STACK.md`
- Architecture changed: Check `ARCHITECTURE.md`

### Response Template:
```
Detected change in: [File Path]

Documentation update needed:
├── FEATURES.md: [Action]
├── TECHNOLOGY-STACK.md: [Action]
├── API.md: [Action]
└── INDEX.md: Update TOC

Will add these to task. Continue?
```

## Files Maintained

### Read (to understand state):
- `docs/tasks.json` - Active tasks
- `docs/.tasks-backup.json` - Backup tasks
- `docs/architecture.md` - System architecture
- `docs/features.md` - Feature list
- `docs/technology-stack.md` - Tech stack
- `docs/how-it-works.md` - Detailed flows
- `docs/api.md` - API reference
- `docs/index.md` - Table of contents

### Write (to maintain state):
- `docs/tasks.json` - Add/remove/update tasks
- `docs/.tasks-backup.json` - Backup created when updated

### Never Touch (by default):
- Actual code files (unless part of task)
- Pre-existing documentation (update as part of tasks)
- Test files (unless explicitly requested)