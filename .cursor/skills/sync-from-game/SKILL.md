---
name: sync-from-game
description: Export GridDungeon.Core sources from griddungeon-game into this mirror (read-only on game repo). Use when user asks to sync, update mirror, refresh Core export, or after game Core merge.
---

# Sync Core from griddungeon-game

## When to use

- User says sync / update mirror / refresh export
- After merging Core changes in **griddungeon-game**
- Before tagging mirror release

## Preconditions

- Sibling checkout: `../griddungeon-game` (or pass `-GameRepoRoot`)
- Game path exists: `Assets/Scripts/Core/**/*.cs`

## Commands

From **griddungeon-core** repo root:

```powershell
./scripts/sync-from-game.ps1
```

Custom game path:

```powershell
./scripts/sync-from-game.ps1 -GameRepoRoot D:\MiraGameDev\griddungeon-game
```

Dry run:

```powershell
./scripts/sync-from-game.ps1 -WhatIf
```

## Agent workflow

1. **Do not** edit griddungeon-game unless user explicitly scoped game work
2. Run sync script from this repo
3. Verify `SYNC_STAMP.json` updated (`gameCommit`, `fileCount`)
4. `dotnet build GridDungeon.Core.sln -c Release`
5. `git diff` — expect `src/**/*.cs` + `SYNC_STAMP.json` only (unless script/tooling changed)
6. If user asked to commit: message `sync: mirror Core from griddungeon-game @ <short-sha>`

## Stale cleanup

Script removes `src/**/*.cs` deleted in game Core.

## Wrong repo for feature work

New simulators/DTOs → game `Assets/Scripts/Core/` first, then sync. See `mirror-source-of-truth.mdc`.
