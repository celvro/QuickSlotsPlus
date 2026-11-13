<#
.SYNOPSIS
    Builds QuickSlotsPlus, packages it, and uploads it to Nexus Mods as a new version of an existing file.

.DESCRIPTION
    Without -Publish this is a dry run: it builds, zips, and prints what would be uploaded.
    Nothing leaves the machine until -Publish is passed.

    The API key is read from the NEXUSMODS_API_KEY environment variable (process, then user scope).
    Per-game Nexus IDs live in nexus.json next to this script.

    Keep this file ASCII-only: Windows PowerShell 5.1 reads BOM-less scripts as ANSI.

.EXAMPLE
    .\Deploy-NexusMods.ps1                      # dry run, both games
    .\Deploy-NexusMods.ps1 -ListFiles           # show the file IDs to put in nexus.json
    .\Deploy-NexusMods.ps1 -Game Subnautica -Publish
#>
[CmdletBinding()]
param(
    [ValidateSet('All', 'Subnautica', 'BelowZero')]
    [string]$Game = 'All',

    # Prepare a release instead of building: write this version into QuickSlotsPlus.cs and
    # release\manifest.json, and turn the README's "### Unreleased" section into this version's.
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$SetVersion,

    # With -SetVersion: optional heading suffix, as in "### 3.1.0 (Nitrox fix)".
    [string]$Title,

    # Actually upload. Without it nothing is sent to Nexus Mods.
    [switch]$Publish,

    # List the mod files (and their IDs) on each configured mod page, then exit.
    [switch]$ListFiles,

    # Package whatever is already in release\<Game>\ instead of rebuilding.
    [switch]$SkipBuild,

    # Changelog text, one entry per line. Default: the README section for this version.
    [string]$Changelog,

    # Do not add a changelog entry on the mod page.
    [switch]$NoChangelog,

    # Publish even from a dirty working tree or a branch other than main.
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12

$ApiBase = 'https://api.nexusmods.com/v3'
$UserAgent = 'nexus-deploy-script/1.0'
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$Config = Get-Content (Join-Path $PSScriptRoot 'nexus.json') -Raw -Encoding UTF8 | ConvertFrom-Json

function Write-Step([string]$Text) { Write-Host "==> $Text" -ForegroundColor Cyan }

# --- API helpers ---------------------------------------------------------------------------

function Get-ApiKey {
    $key = $env:NEXUSMODS_API_KEY
    if (-not $key) { $key = [Environment]::GetEnvironmentVariable('NEXUSMODS_API_KEY', 'User') }
    if (-not $key) {
        throw "NEXUSMODS_API_KEY is not set. Create a key at https://www.nexusmods.com/settings/api-keys and store it with: setx NEXUSMODS_API_KEY <key>"
    }
    return $key.Trim()
}

function Get-HttpErrorText($ErrorRecord) {
    $text = $ErrorRecord.Exception.Message
    if ($ErrorRecord.ErrorDetails -and $ErrorRecord.ErrorDetails.Message) {
        return "$text - $($ErrorRecord.ErrorDetails.Message)"
    }
    $response = $ErrorRecord.Exception.Response
    if ($response) {
        try {
            $reader = New-Object IO.StreamReader($response.GetResponseStream())
            $body = $reader.ReadToEnd()
            if ($body) { $text = "$text - $body" }
        } catch { }
    }
    return $text
}

function Invoke-Nexus([string]$Method, [string]$Path, $Body) {
    $request = @{
        Method    = $Method
        Uri       = "$ApiBase$Path"
        Headers   = @{ apikey = (Get-ApiKey) }
        UserAgent = $UserAgent
    }
    if ($null -ne $Body) {
        # Windows PowerShell sends string bodies as Latin-1, which mangles non-ASCII changelog text.
        $request.Body = [Text.Encoding]::UTF8.GetBytes(($Body | ConvertTo-Json -Depth 5))
        $request.ContentType = 'application/json'
    }
    try {
        return Invoke-RestMethod @request
    } catch {
        throw "Nexus API $Method $Path failed: $(Get-HttpErrorText $_)"
    }
}

# The v3 API addresses mods by their own ID, not the number in the site URL.
function Get-NexusMod($GameConfig) {
    return (Invoke-Nexus 'GET' "/games/$($GameConfig.gameDomain)/mods/$($GameConfig.modId)").data
}

function Send-Upload([string]$FilePath) {
    $size = (Get-Item $FilePath).Length
    $upload = (Invoke-Nexus 'POST' '/uploads/multipart' @{
        filename   = [IO.Path]::GetFileName($FilePath)
        size_bytes = "$size"
    }).data

    $partSize = [int]$upload.part_size_bytes
    $urls = @($upload.part_presigned_urls)
    $parts = @()
    $stream = [IO.File]::OpenRead($FilePath)
    try {
        for ($i = 0; $i -lt $urls.Count; $i++) {
            $buffer = New-Object byte[] $partSize
            $read = 0
            while ($read -lt $partSize) {
                $n = $stream.Read($buffer, $read, $partSize - $read)
                if ($n -le 0) { break }
                $read += $n
            }
            if ($read -lt $partSize) {
                $trimmed = New-Object byte[] $read
                [Array]::Copy($buffer, $trimmed, $read)
                $buffer = $trimmed
            }
            Write-Host "    part $($i + 1)/$($urls.Count) ($read bytes)"
            try {
                $response = Invoke-WebRequest -Method Put -Uri $urls[$i] -Body $buffer `
                    -ContentType 'application/octet-stream' -UseBasicParsing
            } catch {
                throw "Uploading part $($i + 1) failed: $(Get-HttpErrorText $_)"
            }
            $etag = "$($response.Headers['ETag'])".Replace('"', '')
            if (-not $etag) { throw "No ETag returned for part $($i + 1)" }
            $parts += "  <Part>`n    <PartNumber>$($i + 1)</PartNumber>`n    <ETag>$etag</ETag>`n  </Part>"
        }
    } finally {
        $stream.Dispose()
    }

    $xml = "<CompleteMultipartUpload>`n$($parts -join "`n")`n</CompleteMultipartUpload>"
    try {
        Invoke-WebRequest -Method Post -Uri $upload.complete_presigned_url -Body $xml `
            -ContentType 'application/xml' -UseBasicParsing | Out-Null
    } catch {
        throw "Completing the multipart upload failed: $(Get-HttpErrorText $_)"
    }

    Invoke-Nexus 'POST' "/uploads/$($upload.id)/finalise" | Out-Null

    # Nexus scans the file before it can be attached to a mod.
    $delay = 2.0
    for ($attempt = 0; $attempt -lt 60; $attempt++) {
        $state = (Invoke-Nexus 'GET' "/uploads/$($upload.id)").data.state
        Write-Host "    upload state: $state"
        if ($state -eq 'available') { return $upload.id }
        Start-Sleep -Seconds ([Math]::Min($delay, 30))
        $delay *= 1.5
    }
    throw "Upload $($upload.id) was not processed in time. Nothing was attached to the mod page."
}

# --- Local helpers -------------------------------------------------------------------------

function Get-ModVersion {
    $source = Get-Content (Join-Path $RepoRoot 'QuickSlotsPlus.cs') -Raw
    if ($source -notmatch 'versionString\s*=\s*"([^"]+)"') { throw 'versionString not found in QuickSlotsPlus.cs' }
    $version = $Matches[1]
    $manifest = Get-Content (Join-Path $RepoRoot 'release\manifest.json') -Raw | ConvertFrom-Json
    if ($manifest.version_number -ne $version) {
        throw "Version mismatch: QuickSlotsPlus.cs says $version, release\manifest.json says $($manifest.version_number)."
    }
    return $version
}

# Reads the bullets under "### <version>" in README.md. A 3.1.0 release matches "### 3.1" as well.
function Get-ReadmeChangelog([string]$Version) {
    $names = @($Version)
    if ($Version -match '^(\d+\.\d+)\.0$') { $names += $Matches[1] }
    $entries = @()
    $inSection = $false
    foreach ($line in (Get-Content (Join-Path $RepoRoot 'README.md') -Encoding UTF8)) {
        if ($line -match '^(#{2,3})\s+(\S+)') {
            if ($inSection) { break }
            $inSection = ($Matches[1] -eq '###') -and ($names -contains $Matches[2])
            continue
        }
        if ($inSection -and $line -match '^\s*[\*\-]\s+(.+)$') { $entries += $Matches[1].Trim() }
    }
    return ($entries -join "`n")
}

# Rewrites a file through $Transform, keeping its BOM and line endings as they were.
function Edit-TextFile([string]$Path, [scriptblock]$Transform) {
    $bytes = [IO.File]::ReadAllBytes($Path)
    $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
    $encoding = New-Object Text.UTF8Encoding($hasBom)
    $old = [IO.File]::ReadAllText($Path, $encoding)
    $new = & $Transform $old
    if ($new -ceq $old) { return $false }
    [IO.File]::WriteAllText($Path, $new, $encoding)
    return $true
}

function Set-ReleaseVersion([string]$Version, [string]$Title) {
    $csPath = Join-Path $RepoRoot 'QuickSlotsPlus.cs'
    $manifestPath = Join-Path $RepoRoot 'release\manifest.json'
    $readmePath = Join-Path $RepoRoot 'README.md'
    $releaseReadmePath = Join-Path $RepoRoot 'release\README.md'

    $heading = "### $Version"
    if ($Title) { $heading = "$heading ($Title)" }

    # Check the changelog first so a refusal leaves every file untouched.
    $readme = [IO.File]::ReadAllText($readmePath)
    $hasUnreleased = $readme -match '(?m)^### Unreleased[ \t]*\r?$'
    if (-not $hasUnreleased -and -not (Get-ReadmeChangelog $Version)) {
        throw "README.md has neither a '### Unreleased' section nor one for $Version. Write the changelog first."
    }

    if (Edit-TextFile $csPath { param($t) [regex]::Replace($t, '(versionString\s*=\s*")[^"]+(")', "`${1}$Version`${2}") }) {
        Write-Host "    QuickSlotsPlus.cs       versionString = $Version"
    }
    if (Edit-TextFile $manifestPath { param($t) [regex]::Replace($t, '("version_number"\s*:\s*")[^"]+(")', "`${1}$Version`${2}") }) {
        Write-Host "    release\manifest.json   version_number = $Version"
    }
    if ($hasUnreleased) {
        Edit-TextFile $readmePath { param($t) [regex]::Replace($t, '(?m)^### Unreleased[ \t]*(?=\r?$)', $heading) } | Out-Null
        Write-Host "    README.md               '### Unreleased' -> '$heading'"
    }

    # release\README.md is the shorter store readme with its own copy of the changelog.
    if (Test-Path $releaseReadmePath) {
        $bullets = (Get-ReadmeChangelog $Version) -split "`n" | ForEach-Object { "* $_" }
        $escaped = [regex]::Escape("### $Version")
        $added = Edit-TextFile $releaseReadmePath {
            param($t)
            if ($t -match "(?m)^$escaped(\s|$)" -or $t -notmatch '(?m)^## Changes[ \t]*\r?$') { return $t }
            $nl = "`n"
            if ($t.Contains("`r`n")) { $nl = "`r`n" }
            $section = "## Changes$nl$nl$heading$nl$nl$($bullets -join $nl)"
            return [regex]::Replace($t, '(?m)^## Changes[ \t]*(?=\r?$)', $section.Replace('$', '$$'))
        }
        if ($added) { Write-Host "    release\README.md       added '$heading' section" }
    }
}

function Get-MSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $found = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
        if ($found) { return $found }
    }
    $onPath = Get-Command msbuild -ErrorAction SilentlyContinue
    if ($onPath) { return $onPath.Source }
    throw 'MSBuild not found. Install Visual Studio or the Build Tools.'
}

function Invoke-Build([string]$Configuration, $GameConfig) {
    $dll = Join-Path $RepoRoot "release\$Configuration\BepInEx\plugins\$($GameConfig.pluginFolder)\QuickSlotsPlus.dll"
    $started = Get-Date
    & (Get-MSBuild) (Join-Path $RepoRoot 'QuickSlotsPlus.csproj') /restore /t:Rebuild "/p:Configuration=$Configuration" /v:minimal /nologo
    if ($LASTEXITCODE -ne 0) { throw "Build failed for $Configuration (exit code $LASTEXITCODE)." }
    if (-not (Test-Path $dll) -or (Get-Item $dll).LastWriteTime -lt $started.AddSeconds(-2)) {
        throw "Build succeeded but $dll was not refreshed, so the package would ship a stale DLL."
    }
}

# Compress-Archive on Windows PowerShell writes backslash entry names, which other platforms
# and some mod managers do not treat as folders. Write the entries by hand instead.
function New-ReleaseZip([string]$Configuration, $GameConfig, [string]$Version) {
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem

    $root = Join-Path $RepoRoot "release\$Configuration"
    $pluginDir = Join-Path $root "BepInEx\plugins\$($GameConfig.pluginFolder)"
    if (-not (Test-Path (Join-Path $pluginDir 'QuickSlotsPlus.dll'))) { throw "No built DLL in $pluginDir." }

    $zipPath = Join-Path $root $GameConfig.zipName.Replace('{version}', $Version)
    if (Test-Path $zipPath) { Remove-Item $zipPath }

    $files = Get-ChildItem $pluginDir -Recurse -File
    $zip = [IO.Compression.ZipFile]::Open($zipPath, [IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($file in $files) {
            $entryName = $file.FullName.Substring($root.Length + 1).Replace('\', '/')
            [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $file.FullName, $entryName, [IO.Compression.CompressionLevel]::Optimal) | Out-Null
        }
    } finally {
        $zip.Dispose()
    }
    Write-Host "    $zipPath ($($files.Count) files, $((Get-Item $zipPath).Length) bytes)"
    return $zipPath
}

function Assert-ReleasableTree {
    $branch = (& git -C $RepoRoot rev-parse --abbrev-ref HEAD).Trim()
    $dirty = & git -C $RepoRoot status --porcelain
    $problems = @()
    if ($branch -ne 'main') { $problems += "on branch '$branch', not main" }
    if ($dirty) { $problems += 'the working tree has uncommitted changes' }
    if ($problems.Count -eq 0) { return }
    $message = "Not a clean release state: $($problems -join '; ')."
    if ($Publish -and -not $Force) { throw "$message Pass -Force to publish anyway." }
    Write-Warning $message
}

# --- Main ----------------------------------------------------------------------------------

$selected = @('Subnautica', 'BelowZero')
if ($Game -ne 'All') { $selected = @($Game) }

if ($ListFiles) {
    foreach ($name in $selected) {
        $gameConfig = $Config.games.$name
        if (-not $gameConfig.modId) { Write-Warning "$name`: modId is empty in nexus.json, skipped."; continue }
        $mod = Get-NexusMod $gameConfig
        Write-Step "$name - $($mod.name) (nexusmods.com/$($gameConfig.gameDomain)/mods/$($gameConfig.modId))"
        (Invoke-Nexus 'GET' "/mods/$($mod.id)/files").data.mod_files |
            Select-Object @{ n = 'fileId'; e = { $_.id } }, name, is_active, versions_count, last_file_uploaded_at |
            Format-Table -AutoSize | Out-String | Write-Host
    }
    return
}

if ($SetVersion) {
    if ($Publish) { throw '-SetVersion only prepares the release. Review and commit it, then run -Publish separately.' }
    Write-Step "Preparing release $SetVersion"
    Set-ReleaseVersion $SetVersion $Title
    Write-Step 'Review the diff and commit, then run a dry run and -Publish.'
    return
}

$version = Get-ModVersion
Write-Step "QuickSlotsPlus $version"
Assert-ReleasableTree

if (-not $NoChangelog -and -not $Changelog) { $Changelog = Get-ReadmeChangelog $version }
if ($NoChangelog) { $Changelog = '' }
if ($Changelog) {
    Write-Host "Changelog for ${version}:"
    $Changelog -split "`n" | ForEach-Object { Write-Host "    - $_" }
} elseif (-not $NoChangelog) {
    $message = "README.md has no '### $version' section with bullets, so there is no changelog."
    if ($Publish) { throw "$message Rename '### Unreleased', pass -Changelog, or pass -NoChangelog." }
    Write-Warning $message
}

# Explicitly requested games must be configured. With -Game All, unconfigured ones are skipped.
$targets = @()
foreach ($name in $selected) {
    $gameConfig = $Config.games.$name
    if (-not $gameConfig.fileId) {
        $message = "$name`: fileId is empty in nexus.json (run with -ListFiles to find it)."
        if ($Publish -and $Game -ne 'All') { throw $message }
        if ($Publish) { Write-Warning "$message Skipped."; continue }
        Write-Warning "$message It would be skipped on -Publish."
    }
    $targets += $name
}
if ($targets.Count -eq 0) { throw 'No game is configured for upload. Fill in nexus.json first.' }
if ($Publish) { Get-ApiKey | Out-Null }

$packages = @{}
foreach ($name in $targets) {
    $gameConfig = $Config.games.$name
    if (-not $SkipBuild) {
        Write-Step "Building $name"
        Invoke-Build $name $gameConfig
    }
    Write-Step "Packaging $name"
    $packages[$name] = New-ReleaseZip $name $gameConfig $version
}

if (-not $Publish) {
    Write-Step 'Dry run complete. Nothing was uploaded. Re-run with -Publish to upload.'
    return
}

$changelogDone = @{}
foreach ($name in $targets) {
    $gameConfig = $Config.games.$name
    Write-Step "Uploading $name"
    $uploadId = Send-Upload $packages[$name]

    $created = (Invoke-Nexus 'POST' "/mod-files/$($gameConfig.fileId)/versions" @{
        upload_id             = $uploadId
        name                  = $gameConfig.displayName
        version               = $version
        file_category         = $gameConfig.category
        archive_existing_file = [bool]$gameConfig.archiveExisting
        update_mod_version    = [bool]$gameConfig.updateModVersion
    }).data
    Write-Host "    published version id $($created.version.id) on file '$($created.file.name)'"

    # The changelog belongs to the mod page, so add it once even if both games share a page.
    $modKey = "$($gameConfig.gameDomain)/$($gameConfig.modId)"
    if ($Changelog -and $gameConfig.modId -and -not $changelogDone[$modKey]) {
        $mod = Get-NexusMod $gameConfig
        Invoke-Nexus 'POST' "/mods/$($mod.id)/changelogs" @{ version = $version; changelog = $Changelog } | Out-Null
        $changelogDone[$modKey] = $true
        Write-Host '    changelog added'
    }
    Write-Host "    https://www.nexusmods.com/$($gameConfig.gameDomain)/mods/$($gameConfig.modId)?tab=files"
}
Write-Step "Published QuickSlotsPlus $version to Nexus Mods."
