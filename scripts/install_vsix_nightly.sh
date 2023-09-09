#!/bin/bash
set -e

# Fetch
REPO="Azure/bicep"
lastRunId=$(gh run list -R $REPO --branch main --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId")
tmpDir=$(mktemp -d)
gh run download -R $REPO $lastRunId -n vscode-bicep.vsix --dir $tmpDir

# Install
code --install-extension "$tmpDir/vscode-bicep.vsix" --force

# Cleanup
rm -Rf $tmpDir