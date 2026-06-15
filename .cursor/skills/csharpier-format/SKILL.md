---
name: csharpier-format
description: Format C# with CSharpier in griddungeon-core. Use when user asks to run CSharpier, format .cs files, or before commit.
---

# CSharpier — griddungeon-core

## Root

Repository root: folder with `GridDungeon.Core.csproj` and `.config/dotnet-tools.json`.

## Commands

```powershell
dotnet tool restore
dotnet csharpier format src/SomeFile.cs
dotnet csharpier format src/
dotnet csharpier format .
```

## Agent workflow

1. Scope to changed paths unless user asked for all `.cs`
2. Run from repo root
3. If CLI missing: ask user to install local tool or use Cursor Format Document (CSharpier extension)

## Out of scope

`.md`, `.yml`, `.json` — C# only.
