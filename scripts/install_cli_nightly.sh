#!/bin/bash
set -e

usage="Usage: ./install_cli_nightly.sh <platform> <arch>"
platform=${1:?"Missing platform (e.g. 'osx' or 'linux'). ${usage}"}
arch=${2:?"Missing arch (e.g. 'x64' or 'arm64'). ${usage}"}

if ! command -v gh > /dev/null; then
  echo "Please install the GitHub CLI: https://cli.github.com/"
  exit 1
fi

# Fetch
REPO="Azure/bicep"
lastRunId=$(gh run list -R $REPO --branch main --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId")
tmpDir=$(mktemp -d)
gh run download -R $REPO $lastRunId -n "bicep-release-$platform-$arch" --dir $tmpDir

# Install
AZCLI_BIN_DIR="$HOME/.azure/bin"
if [[ $platform == "osx" ]]; then
  # Ad-hoc sign the binary
  codesign -s - "$tmpDir/bicep"
fi

mkdir -p $AZCLI_BIN_DIR
mv "$tmpDir/bicep" "$AZCLI_BIN_DIR/bicep"
chmod +x "$AZCLI_BIN_DIR/bicep"

# Cleanup
rm -Rf $tmpDir
