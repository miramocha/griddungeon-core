# GridDungeon.Core (mirror)

Plain **.NET** mirror of [`griddungeon-game` `Assets/Scripts/Core`](https://github.com/miramocha/griddungeon-game/tree/main/Assets/Scripts/Core).

No Unity metadata (`.meta`, `.asmdef`). Same namespaces and folder layout as the game repo. **Source of truth stays in griddungeon-game** — this repo is an export for reference, `dotnet build`, and future non-Unity consumers.

## What is included

- Combat / map / exploration simulators and DTOs
- Save schema shapes and mappers
- Maze generation algorithms
- Story **data shapes** only (no Campaign executors)

## What is not included

| Left in game repo | Why |
|---|---|
| `GridDungeon.Campaign` | Stratum/story behavior |
| `GridDungeon.Runtime` / UI | Unity lifecycle and presenters |
| Unity Edit Mode tests | Unity Test Runner harness (test port is future work) |
| Content ScriptableObjects | Runtime assets |

## Build

```powershell
dotnet build GridDungeon.Core.sln -c Release
```

Target: **netstandard2.1** (Unity-friendly if you later embed as UPM). Lang **C# 9**, nullable enabled — matches Unity 6 game repo.

## Sync from game repo

From this repo root (read-only on `griddungeon-game`):

```powershell
./scripts/sync-from-game.ps1
```

Custom game checkout path:

```powershell
./scripts/sync-from-game.ps1 -GameRepoRoot D:\MiraGameDev\griddungeon-game
```

Dry run:

```powershell
./scripts/sync-from-game.ps1 -WhatIf
```

Writes `SYNC_STAMP.json` with game commit SHA and file counts. Commit mirror changes after sync.

See [SYNC.md](SYNC.md) for workflow.

## Consume as reference

Clone and browse `src/` — layout mirrors game `Assets/Scripts/Core`.

## Consume as library

```xml
<ProjectReference Include="path/to/griddungeon-core/GridDungeon.Core.csproj" />
```

Or pack locally:

```powershell
dotnet pack GridDungeon.Core.csproj -c Release
```

NuGet publish to a registry is optional and not set up by default.

## Cursor (rules / skills)

Agent guidance lives in [`.cursor/`](.cursor/README.md):

- **mirror-source-of-truth** — edit game Core, sync here
- **core-boundaries** — what belongs in Core vs Campaign/Runtime
- **sync-from-game** skill — export workflow

## Related docs

- [Core assembly improvement plan](https://github.com/miramocha/griddungeon-design-docs/blob/main/docs/plans/core-assembly-improvement-plan.md) (game repo assembly strategy)
- [05-class-design](https://github.com/miramocha/griddungeon-design-docs/blob/main/docs/05-class-design.md)
