# Sync workflow

## Authority

| Repo | Role |
|---|---|
| **griddungeon-game** | Canonical `Assets/Scripts/Core` — all edits happen here |
| **griddungeon-core** (this repo) | Export snapshot — no direct feature work on `src/` |

## When to sync

- After merging Core changes in game repo
- Before tagging a mirror release
- When verifying `dotnet build` on latest rules

## Steps

1. Ensure game repo is on the commit you want to export.
2. From **this repo** root:

   ```powershell
   ./scripts/sync-from-game.ps1
   ```

3. Review diff (`git diff`). Expect only `.cs` under `src/` plus `SYNC_STAMP.json`.
4. Build:

   ```powershell
   dotnet build -c Release
   ```

5. Commit mirror:

   ```text
   sync: mirror Core from griddungeon-game @ <short-sha>
   ```

## SYNC_STAMP.json

Machine-readable record of last export:

- `gameCommit` — full SHA from game repo `HEAD` at sync time
- `fileCount` — `.cs` files in Core
- `syncedAtUtc` — export timestamp

## Stale file cleanup

`sync-from-game.ps1` removes `.cs` under `src/` that no longer exist in game Core (e.g. renamed/deleted types).

## Tests (future)

Unity Edit Mode tests remain in game repo. Porting Core-only fixtures to `dotnet test` + NUnit is a separate phase — not required for reference mirror.

## Do not

- Edit game repo from this workflow (export is read-only copy)
- Commit `.meta` or `.asmdef` into this repo
- Treat Campaign/Runtime copies as in scope for this mirror
