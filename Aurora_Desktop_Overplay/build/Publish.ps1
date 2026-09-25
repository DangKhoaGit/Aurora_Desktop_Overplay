param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "0.1.0",
    [switch]$BuildInstaller,
    [string]$SigningCertificatePath
)

$ErrorActionPreference = "Stop"
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$artifactsRoot = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot "artifacts"))
$publishDirectory = Join-Path $artifactsRoot "publish\$Runtime"
$releaseDirectory = Join-Path $artifactsRoot "release"
$project = Join-Path $repositoryRoot "src\Aurora.Desktop.Overlay.App\Aurora.Desktop.Overlay.App.csproj"
$solution = Join-Path $repositoryRoot "Aurora.Desktop.Overlay.slnx"

function Find-SignTool {
    $kitsRoot = Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\bin"
    $candidate = Get-ChildItem -LiteralPath $kitsRoot -Filter "signtool.exe" -Recurse -ErrorAction SilentlyContinue |
        Where-Object FullName -Match '\\x64\\signtool\.exe$' |
        Sort-Object FullName -Descending |
        Select-Object -First 1
    if (-not $candidate) { throw "Windows SDK signtool.exe was not found." }
    return $candidate.FullName
}

function Invoke-CodeSigning([string]$FilePath) {
    if (-not $SigningCertificatePath) { return }
    if (-not (Test-Path -LiteralPath $SigningCertificatePath)) { throw "Signing certificate was not found." }
    if (-not $env:ADO_SIGNING_CERT_PASSWORD) { throw "ADO_SIGNING_CERT_PASSWORD is required for signing." }

    $signTool = Find-SignTool
    & $signTool sign /fd SHA256 /td SHA256 /tr "http://timestamp.digicert.com" `
        /f $SigningCertificatePath /p $env:ADO_SIGNING_CERT_PASSWORD $FilePath
    if ($LASTEXITCODE -ne 0) { throw "Code signing failed for $FilePath." }
    & $signTool verify /pa $FilePath
    if ($LASTEXITCODE -ne 0) { throw "Signature verification failed for $FilePath." }
}

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

Invoke-CodeSigning (Join-Path $publishDirectory "AuroraDesktopOverlay.exe")

$portableArchive = Join-Path $releaseDirectory "AuroraDesktopOverlay-$Version-$Runtime-portable.zip"
if (Test-Path -LiteralPath $portableArchive) { Remove-Item -LiteralPath $portableArchive -Force }
Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $portableArchive -CompressionLevel Optimal

if ($BuildInstaller) {
    $uninstallKeys = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
    )
    $registeredLocations = $uninstallKeys |
        ForEach-Object { Get-ChildItem -LiteralPath $_ -ErrorAction SilentlyContinue } |
        Get-ItemProperty |
        Where-Object DisplayName -Like "Inno Setup*" |
        ForEach-Object { Join-Path $_.InstallLocation "ISCC.exe" }
    $compilerCandidates = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    ) + $registeredLocations
    $compiler = $compilerCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not $compiler) { throw "Inno Setup 6 was not found." }
    & $compiler "/DMyAppVersion=$Version" "/DPublishDir=$publishDirectory" `
        "/DOutputDir=$releaseDirectory" (Join-Path $PSScriptRoot "installer.iss")
    if ($LASTEXITCODE -ne 0) { throw "Installer build failed." }
    Invoke-CodeSigning (Join-Path $releaseDirectory "AuroraDesktopOverlay-$Version-$Runtime-setup.exe")
}

Get-ChildItem -LiteralPath $releaseDirectory -File |
    Where-Object Extension -In ".zip", ".exe" |
    ForEach-Object {
        $hash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $($_.Name)" | Set-Content -LiteralPath "$($_.FullName).sha256" -Encoding ascii
    }

Write-Output "Release artifacts: $releaseDirectory"
