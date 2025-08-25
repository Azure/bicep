[cmdletbinding()]
param(
   [string]$RunId,
   [string]$Branch,
   [string]$Repo
)

$ErrorActionPreference="Stop"

if ((Get-Command "gh" -ErrorAction SilentlyContinue) -eq $null) { 
    throw "Please install the GitHub CLI: https://cli.github.com/"
}

# Fetch
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
& gh run download -R $Repo $RunId -n "vscode-bicep.vsix" --dir $tmpDir; if(!$?) { throw }

# Install
& code --install-extension "$tmpDir/vscode-bicep.vsix" --force; if(!$?) { throw }

echo "Installed Bicep VSCode extension from https://github.com/$Repo/actions/runs/$RunId"

# Cleanup
Remove-Item $tmpDir -Recurse