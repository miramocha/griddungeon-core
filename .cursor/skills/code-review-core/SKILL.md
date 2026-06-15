---
name: code-review-core
description: Review pure C# GridDungeon.Core mirror diffs — simulators, DTOs, sync exports. Use for PR review or post-commit review in griddungeon-core.
---

# Code review — GridDungeon.Core mirror

Auto-enables caveman mode (full) for review output.

## Scope

- `src/**/*.cs` — simulators, models, save DTOs, maze gen
- Sync commits: `SYNC_STAMP.json`, script changes, CI
- **Not** Unity Runtime/UI/Campaign (other repos)

## Checklist

### Mirror hygiene

- [ ] Feature logic change belongs in **game repo** first? (`mirror-source-of-truth.mdc`)
- [ ] No `UnityEngine`, `MonoBehaviour`, `.meta` in diff
- [ ] `SYNC_STAMP.json` matches claimed game SHA (sync commits)

### Core boundaries (`core-boundaries.mdc`)

- [ ] No story executors / campaign resolvers in Core
- [ ] DTO vs behavior split respected

### C# (`csharp-language.mdc`)

- [ ] No `init`, `record`, file-scoped namespaces in `src/`
- [ ] Nullable consistent

### Correctness

- [ ] Simulator deterministic / pure where expected
- [ ] No duplicate formulas that exist elsewhere in Core
- [ ] Public API breaks documented if mirror consumed as library

### Tests / build

- [ ] `dotnet build -c Release` passes (or user reported green)

## Severity

| Level | Examples |
|---|---|
| **Blocker** | Build break, wrong formula, mirror-only feature that will be overwritten |
| **Should fix** | Boundary violation, C# 10+ in synced code, missing sync stamp |
| **Nit** | Naming drift, comment noise |

## Output

```markdown
## Review

### Blocker
- file:line — issue — fix

### Should fix
- …

### Nit
- …
```

## Sync-only mega-diff

For export touching 100+ files: verify stamp + build + spot-check unexpected paths — skip per-file nit on identical game copies.
