# Cursor config (griddungeon-core)

Rules and skills for the **plain .NET mirror** of `griddungeon-game` `Assets/Scripts/Core`.

## Authority

| Repo | Role |
|---|---|
| [griddungeon-game](https://github.com/miramocha/griddungeon-game) | Canonical Core source — feature edits |
| **griddungeon-core** (this repo) | Export + `dotnet build` + reference |

See [SYNC.md](../SYNC.md) and rule `mirror-source-of-truth.mdc`.

## Rules (`.cursor/rules/`)

| Rule | Purpose |
|---|---|
| `mirror-source-of-truth.mdc` | Never edit `src/` for game rules; sync from game repo |
| `core-boundaries.mdc` | What belongs in Core vs Campaign/Runtime (when editing game repo) |
| `csharp-language.mdc` | C# 9 subset — Unity parity for synced code |
| `csharp-naming.mdc` / `csharp-formatting.mdc` | Style aligned with game repo |
| `clean-code-principles.mdc` | SRP, DRY, KISS |
| `git-commit-agent-workflow.mdc` | Agent commit order |
| `pre-commit-csharpier-format.mdc` / `post-commit-csharp-code-review.mdc` | Format + review on commits |

## Skills (`.cursor/skills/`)

| Skill | Use when |
|---|---|
| `sync-from-game` | Export latest Core from game repo |
| `csharpier-format` | Format `src/` before commit |
| `code-review-core` | Review pure C# mirror / sync diffs |

## Agents (`.cursor/agents/`)

`fresh-reviewer.md` — independent post-commit review for this repo (no Unity context).

## Not in this repo

Unity-only rules (UITK, Test Runner, `.meta`, Editor-open tests) stay in **griddungeon-game** / **griddungeon-design-docs**.
