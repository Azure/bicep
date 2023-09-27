#!/bin/bash
set -e

while [ $# -gt 0 ]; do
  case "$1" in
    --run-id)   runId="${2}"; shift;;
    --branch)   branch="${2}"; shift;;
    *)          echo "Unrecognized argument \"${1}\"."; exit 1;;
  esac
  shift
done

if ! command -v gh > /dev/null; then
  echo "Please install the GitHub CLI: https://cli.github.com/"; exit 1;
fi

case "$(uname -s)" in
  Linux*)     platform=linux;;
  Darwin*)    platform=osx;;
  *)          echo "Unsupported platform '$(uname -s)'"; exit 1;;
esac

case "$(uname -m)" in
  x86_64)     arch="x64" ;;
  arm64)      arch="arm64" ;;
  aarch64)    arch="arm64" ;;
  *)          echo "Unsupported architecture '$(uname -m)'"; exit 1;;
esac

# Fetch
REPO="Azure/bicep"
if [ -z "$branch" ]; then
  branch=main
fi
if [ -z "$runId" ]; then
  runId=$(gh run list -R $REPO --branch $branch --workflow build --status success -L 1 --json databaseId -q ".[0].databaseId")
fi
tmpDir=$(mktemp -d)
gh run download -R $REPO $runId -n "bicep-release-$platform-$arch" --dir $tmpDir

# Install
if [[ $platform == "osx" ]]; then
  # Ad-hoc sign the binary
  codesign -s - "$tmpDir/bicep"
fi

AZCLI_BIN_DIR="$HOME/.azure/bin"
mkdir -p $AZCLI_BIN_DIR
mv "$tmpDir/bicep" "$AZCLI_BIN_DIR/bicep"
chmod +x "$AZCLI_BIN_DIR/bicep"

version=$("$AZCLI_BIN_DIR/bicep" --version | sed 's/^.* \([0-9]*\.[0-9]*\.[0-9]*\) .*/\1/')
echo "Installed Bicep $version from https://github.com/Azure/bicep/actions/runs/$runId to $AZCLI_BIN_DIR/bicep"

# Cleanup
rm -Rf $tmpDir