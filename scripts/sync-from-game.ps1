#Requires -Version 5.1
<#
.SYNOPSIS
  Copy GridDungeon.Core sources from griddungeon-game into this mirror repo.

.DESCRIPTION
  Read-only export: does not modify griddungeon-game.
  Copies Assets/Scripts/Core/**/*.cs into src/ (no .meta, no .asmdef).

.PARAMETER GameRepoRoot
  Path to griddungeon-game checkout. Defaults to sibling ../griddungeon-game.

.PARAMETER DryRun
  List files that would copy without writing.
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string] $GameRepoRoot = "",
    [switch] $DryRun
)

$ErrorActionPreference = "Stop"
$ScriptDir = $PSScriptRoot
if ([string]::IsNullOrEmpty($ScriptDir)) {
    $ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
}
$MirrorRoot = Split-Path $ScriptDir -Parent
if ([string]::IsNullOrWhiteSpace($GameRepoRoot)) {
    $GameRepoRoot = Join-Path (Split-Path $MirrorRoot -Parent) "griddungeon-game"
}
$SourceRoot = Join-Path $GameRepoRoot "Assets\Scripts\Core"
$DestRoot = Join-Path $MirrorRoot "src"

if (-not (Test-Path $SourceRoot)) {
    throw "Core source not found: $SourceRoot. Pass -GameRepoRoot to your griddungeon-game checkout."
}

$sourceFiles = Get-ChildItem -Path $SourceRoot -Filter "*.cs" -Recurse -File
if ($sourceFiles.Count -eq 0) {
    throw "No .cs files under $SourceRoot"
}

Write-Host "Source: $SourceRoot"
Write-Host "Dest:   $DestRoot"
Write-Host "Files:  $($sourceFiles.Count)"

$copied = 0
foreach ($file in $sourceFiles) {
    $relative = $file.FullName.Substring($SourceRoot.Length).TrimStart("\", "/")
    $destPath = Join-Path $DestRoot $relative
    $destDir = Split-Path $destPath -Parent

    if ($DryRun) {
        Write-Host "[dry-run] $relative"
        continue
    }

    if ($PSCmdlet.ShouldProcess($relative, "copy")) {
        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Force -Path $destDir | Out-Null
        }
        Copy-Item -Path $file.FullName -Destination $destPath -Force
        $copied++
    }
}

if ($DryRun) {
    return
}

# Remove mirror files no longer present in game Core (stale export cleanup).
$destFiles = Get-ChildItem -Path $DestRoot -Filter "*.cs" -Recurse -File -ErrorAction SilentlyContinue
$sourceRelative = [System.Collections.Generic.HashSet[string]]::new(
    [StringComparer]::OrdinalIgnoreCase
)
foreach ($file in $sourceFiles) {
    [void]$sourceRelative.Add($file.FullName.Substring($SourceRoot.Length).TrimStart("\", "/"))
}

$removed = 0
foreach ($destFile in $destFiles) {
    $relative = $destFile.FullName.Substring($DestRoot.Length).TrimStart("\", "/")
    if (-not $sourceRelative.Contains($relative)) {
        if ($PSCmdlet.ShouldProcess($relative, "remove stale")) {
            Remove-Item -Path $destFile.FullName -Force
            $removed++
        }
    }
}

$gameHead = $null
if (Test-Path (Join-Path $GameRepoRoot ".git")) {
    Push-Location $GameRepoRoot
    try {
        $gameHead = (git rev-parse HEAD 2>$null)
        $gameBranch = (git rev-parse --abbrev-ref HEAD 2>$null)
    }
    finally {
        Pop-Location
    }
}

$stamp = [ordered]@{
    syncedAtUtc = (Get-Date).ToUniversalTime().ToString("o")
    sourceRepo  = "griddungeon-game"
    sourcePath  = "Assets/Scripts/Core"
    gameRepoRoot = (Resolve-Path $GameRepoRoot).Path
    gameCommit  = $gameHead
    gameBranch  = $gameBranch
    fileCount   = $sourceFiles.Count
    copied      = $copied
    removed     = $removed
}
$stampPath = Join-Path $MirrorRoot "SYNC_STAMP.json"
$stamp | ConvertTo-Json -Depth 4 | Set-Content -Path $stampPath -Encoding UTF8

Write-Host "Copied: $copied  Removed stale: $removed"
Write-Host "Stamp:  $stampPath"
if ($gameHead) {
    Write-Host "Game:   $gameBranch @ $gameHead"
}
