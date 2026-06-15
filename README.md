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

## Godot

GridDungeon.Core is a plain **netstandard2.1** class library with **no Unity or Godot dependencies** — only simulators, DTOs, and save shapes. That makes it a good fit for a Godot 4 **C#** game that wants the same rules without the Unity runtime.

### Prerequisites

- [Godot 4 with .NET support](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html) (Godot 4.5+ needs .NET 8 SDK)
- A C# Godot project (create any node → **Attach Script** → pick C# once to generate `.sln` / `.csproj`)

Use an external IDE (Rider, VS, VS Code) for C# — Godot’s built-in script editor is minimal for C#.

### Reference this repo

**Local project reference** (best while iterating):

1. Clone or submodule `griddungeon-core` next to your Godot project (or anywhere on disk).
2. In your Godot game `.csproj`, add:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/griddungeon-core/GridDungeon.Core.csproj" />
</ItemGroup>
```

3. Build from Godot (**Build** button) or `dotnet build` on the game `.csproj`.

**Packed NuGet** (optional, for sharing a fixed version):

```powershell
dotnet pack path/to/griddungeon-core/GridDungeon.Core.csproj -c Release
```

Then add the produced `.nupkg` as a local feed or publish to a registry and `dotnet add package GridDungeon.Core` from the game project.

### Use Core from Godot scripts

Keep **engine code in Godot** (`Node`, scenes, input, rendering) and call **Core** for rules:

```csharp
using Godot;
using GridDungeon.Simulators;

public partial class CombatBridge : Node
{
    public void ResolveRound(/* your DTOs from Core */)
    {
        // e.g. DamageCalculator, TurnQueueBuilder, MapRevealCalculator
    }
}
```

Mirror the game-repo split:

| Layer | Godot equivalent | Contents |
|---|---|---|
| **GridDungeon.Core** | Project reference to this repo | Formulas, DTOs, maze gen, save schema |
| **GridDungeon.Campaign** | Your game’s campaign/stratum project or folder | Story executors, floor keys, tutorial policy |
| **GridDungeon.Runtime** | Godot `Node`s, autoloads, UI | Scene wiring, save I/O, presentation |

Do **not** add `Godot` usings or `Node` subclasses inside `griddungeon-core` — that repo stays engine-agnostic and syncs from Unity game Core.

### Compatibility notes

- **Target:** `netstandard2.1` — consumable from Godot’s `net8.0` (and similar) game projects.
- **Language:** Core stays **C# 9** (Unity parity). Your Godot scripts can use newer C#; only avoid calling newer syntax across the Core API surface you extend here.
- **No extra NuGet deps** in Core today — nothing else to align in Godot.
- **Rebuild** the Godot solution after pulling or syncing Core changes.
- **Platform:** Godot 4 C# export is desktop-first; mobile/web C# support is still limited — same constraints as any Godot C# project, not specific to Core.

### Staying up to date

Canonical rules still live in **griddungeon-game** `Assets/Scripts/Core`. After game Core changes, run `./scripts/sync-from-game.ps1` here, then rebuild your Godot project against the updated reference.

## Cursor (rules / skills)

Agent guidance lives in [`.cursor/`](.cursor/README.md):

- **mirror-source-of-truth** — edit game Core, sync here
- **core-boundaries** — what belongs in Core vs Campaign/Runtime
- **sync-from-game** skill — export workflow

## Related docs

- [Core assembly improvement plan](https://github.com/miramocha/griddungeon-design-docs/blob/main/docs/plans/core-assembly-improvement-plan.md) (game repo assembly strategy)
- [05-class-design](https://github.com/miramocha/griddungeon-design-docs/blob/main/docs/05-class-design.md)
