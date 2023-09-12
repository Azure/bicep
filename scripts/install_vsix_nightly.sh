#!/bin/bash
set -e

if ! command -v gh > /dev/null; then
  echo "Please install the GitHub CLI: https://cli.github.com/"; exit 1;
fi

# Fetch
REPO="Azure/bicep"
lastRunId=$(gh run list -R $REPO --branch main --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId")
tmpDir=$(mktemp -d)
gh run download -R $REPO $lastRunId -n vscode-bicep.vsix --dir $tmpDir

# Install
code --install-extension "$tmpDir/vscode-bicep.vsix" --force > /dev/null

echo "Installed Bicep VSCode extension from https://github.com/Azure/bicep/actions/runs/$lastRunId"

# Cleanup
rm -Rf $tmpDir
