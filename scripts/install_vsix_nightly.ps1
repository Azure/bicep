[cmdletbinding()]
param(
   [string]$RunId,
   [string]$Branch
)

$ErrorActionPreference="Stop"

if ((Get-Command "gh" -ErrorAction SilentlyContinue) -eq $null) { 
    throw "Please install the GitHub CLI: https://cli.github.com/"
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
& gh run download -R $repo $RunId -n "vscode-bicep.vsix" --dir $tmpDir; if(!$?) { throw }

# Install
& code --install-extension "$tmpDir/vscode-bicep.vsix" --force; if(!$?) { throw }

echo "Installed Bicep VSCode extension from https://github.com/Azure/bicep/actions/runs/$RunId"

# Cleanup
Remove-Item $tmpDir -Recurse