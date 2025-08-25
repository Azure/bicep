[cmdletbinding()]
param(
   [string]$RunId,
   [string]$Branch,
   [string]$Repo,
   [string]$BinaryPath
)

$ErrorActionPreference="Stop"

if ((Get-Command "gh" -ErrorAction SilentlyContinue) -eq $null) { 
    throw "Please install the GitHub CLI: https://cli.github.com/"
}

$platform = [System.Environment]::OSVersion.Platform.ToString().ToLowerInvariant()
switch ($platform) {
    { ($_ -eq "win32nt") } { $platform = "win" }
    default { throw "Unsupported platform '$platform'" }
}

$arch = $env:PROCESSOR_ARCHITECTURE.ToLowerInvariant()
switch ($arch) {
    { ($_ -eq "x64") } { $arch = "x64" }
    { ($_ -eq "amd64") } { $arch = "x64" }
    { $_ -eq "arm64" } { $arch = "arm64" }
    default { throw "Unsupported architecture '$arch'" }
}

# Fetch
if (!$BinaryPath) {
  # Default to ~/.azure/bin
  $BinaryPath = [System.IO.Path]::combine($HOME, ".azure", "bin")
}
if (!$Repo) {
  $Repo = "Azure/bicep"
}
if (!$Branch) {
  $Branch = "main"
}
if (!$RunId) {
  $RunId = & gh run list -R $Repo --branch $Branch --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId"; if(!$?) { throw }
  if (!$RunId) {
    throw "Failed to find a successful build to install from"
  }
}
$tmpDir = [System.IO.Path]::combine([System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName())
& gh run download -R $Repo $RunId -n "bicep-release-$platform-$arch" --dir $tmpDir; if(!$?) { throw }

# Install
$bicepPath = [System.IO.Path]::combine($BinaryPath, "bicep.exe")
New-Item -ItemType Directory -Force -Path $BinaryPath | Out-Null
Move-Item -Path "$tmpDir/bicep.exe" -Destination $bicepPath -Force

$versionStdout = & $bicepPath --version; if(!$?) { throw }
$version = $versionStdout -replace '^.* ([0-9]*\.[0-9]*\.[0-9]*) .*$', '$1'
echo "Installed Bicep $version from https://github.com/$Repo/actions/runs/$RunId to $bicepPath"

# Cleanup
Remove-Item $tmpDir -Recurse