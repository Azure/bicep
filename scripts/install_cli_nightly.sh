#!/bin/bash
set -e

usage="Usage: ./install_cli_nightly.sh <platform> <arch>"
platform=${1:?"Missing platform (e.g. 'osx' or 'linux'). ${usage}"}
arch=${2:?"Missing arch (e.g. 'x64' or 'arm64'). ${usage}"}

# Fetch
REPO="Azure/bicep"
lastRunId=$(gh run list -R $REPO --branch main --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId")
tmpDir=$(mktemp -d)
gh run download -R $REPO $lastRunId -n "bicep-release-$platform-$arch" --dir $tmpDir

# Install
AZCLI_BIN_DIR="$HOME/.azure/bin"
mkdir -p $AZCLI_BIN_DIR
mv "$tmpDir/bicep" "$AZCLI_BIN_DIR/bicep"
chmod +x "$AZCLI_BIN_DIR/bicep"
if [[ $platform == "osx" ]]
then
  # Nightly binaries aren't signed, so OSX requires a gatekeeper exception
  echo "Configuring a Gatekeeper exception requires administrator access"
  sudo spctl --add "$AZCLI_BIN_DIR/bicep"
fi

# Cleanup
rm -Rf $tmpDir