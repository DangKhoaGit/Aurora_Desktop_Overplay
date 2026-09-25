param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "0.1.0",
    [switch]$BuildInstaller
)

$ErrorActionPreference = "Stop"
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$artifactsRoot = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot "artifacts"))
$publishDirectory = Join-Path $artifactsRoot "publish\$Runtime"
$releaseDirectory = Join-Path $artifactsRoot "release"
$project = Join-Path $repositoryRoot "src\Aurora.Desktop.Overlay.App\Aurora.Desktop.Overlay.App.csproj"
$solution = Join-Path $repositoryRoot "Aurora.Desktop.Overlay.slnx"

if (-not $artifactsRoot.StartsWith($repositoryRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Artifact path escaped the repository root."
}

if (Test-Path -LiteralPath $publishDirectory) {
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}
New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null
New-Item -ItemType Directory -Path $releaseDirectory -Force | Out-Null

dotnet restore $solution --locked-mode
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed." }

dotnet publish $project -c $Configuration -r $Runtime --self-contained true --no-restore `
    -p:Version=$Version -p:PublishSingleFile=true -p:PublishTrimmed=false `
    -p:IncludeNativeLibrariesForSelfExtract=true -o $publishDirectory
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed." }

$portableArchive = Join-Path $releaseDirectory "AuroraDesktopOverlay-$Version-$Runtime-portable.zip"
if (Test-Path -LiteralPath $portableArchive) { Remove-Item -LiteralPath $portableArchive -Force }
Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $portableArchive -CompressionLevel Optimal

if ($BuildInstaller) {
    $compilerCandidates = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    )
    $compiler = $compilerCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not $compiler) { throw "Inno Setup 6 was not found." }
    & $compiler "/DMyAppVersion=$Version" "/DPublishDir=$publishDirectory" `
        "/DOutputDir=$releaseDirectory" (Join-Path $PSScriptRoot "installer.iss")
    if ($LASTEXITCODE -ne 0) { throw "Installer build failed." }
}

Get-ChildItem -LiteralPath $releaseDirectory -File |
    Where-Object Extension -In ".zip", ".exe" |
    ForEach-Object {
        $hash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $($_.Name)" | Set-Content -LiteralPath "$($_.FullName).sha256" -Encoding ascii
    }

Write-Output "Release artifacts: $releaseDirectory"
