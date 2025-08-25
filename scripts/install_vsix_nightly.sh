#!/bin/bash
set -e

while [ $# -gt 0 ]; do
  case "$1" in
    --run-id)  runId="${2}"; shift;;
    --branch)  branch="${2}"; shift;;
    --repo)    repo="${2}"; shift;;
    *)         echo "Unrecognized argument \"${1}\"."; exit 1;;
  esac
  shift
done

if ! command -v gh > /dev/null; then
  echo "Please install the GitHub CLI: https://cli.github.com/"; exit 1;
fi

# Fetch
if [ -z "$repo" ]; then
  repo="Azure/bicep"
fi
if [ -z "$branch" ]; then
  branch=main
fi
if [ -z "$runId" ]; then
  runId=$(gh run list -R $repo --branch $branch --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId")
  if [ -z "$runId" ]; then
    echo "Failed to find a successful build to install from"; exit 1;
  fi
fi
tmpDir=$(mktemp -d)
gh run download -R $repo $runId -n "vscode-bicep.vsix" --dir $tmpDir

# Install
code --install-extension "$tmpDir/vscode-bicep.vsix" --force

echo "Installed Bicep VSCode extension from https://github.com/$repo/actions/runs/$runId"

# Cleanup
rm -Rf $tmpDir
