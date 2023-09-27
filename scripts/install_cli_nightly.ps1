[cmdletbinding()]
param(
   [string]$RunId,
   [string]$Branch
)

$ErrorActionPreference="Stop"

if ((Get-Command "gh" -ErrorAction SilentlyContinue) -eq $null) { 
    throw "Please install the GitHub CLI: https://cli.github.com/"
}

$arch = $env:PROCESSOR_ARCHITECTURE.ToLowerInvariant()
switch ($arch) {
    { ($_ -eq "x64") } { $arch = "x64" }
    { ($_ -eq "amd64") } { $arch = "x64" }
    { $_ -eq "arm64" } { $arch = "arm64" }
    default { throw "Unsupported architecture '$arch'" }
}

$platform = [System.Environment]::OSVersion.Platform.ToString().ToLowerInvariant()
switch ($platform) {
    { ($_ -eq "win32nt") } { $platform = "win" }
    default { throw "Unsupported platform '$platform'" }
}

# Fetch
$repo = "Azure/bicep"
if (!$Branch) {
  $Branch = "main"
}
if (!$RunId) {
    $RunId = & gh run list -R $repo --branch $Branch --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId"; if(!$?) { throw }
}
$tmpDir = [System.IO.Path]::combine([System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName())
& gh run download -R $repo $RunId -n "bicep-release-$platform-$arch" --dir $tmpDir; if(!$?) { throw }

# Install
$azCliBinDir = [System.IO.Path]::combine($HOME, ".azure", "bin")
$bicepPath = [System.IO.Path]::combine($azCliBinDir, "bicep.exe")
New-Item -ItemType Directory -Force -Path $azCliBinDir | Out-Null
Move-Item -Path "$tmpDir/bicep.exe" -Destination $bicepPath -Force

$versionStdout = & $bicepPath --version; if(!$?) { throw }
$version = $versionStdout -replace '^.* ([0-9]*\.[0-9]*\.[0-9]*) .*$', '$1'
echo "Installed Bicep $version from https://github.com/Azure/bicep/actions/runs/$RunId to $bicepPath"

# Cleanup
Remove-Item $tmpDir -Recurse