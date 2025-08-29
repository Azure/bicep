#!/bin/bash
set -e

while [ $# -gt 0 ]; do
  case "$1" in
    --run-id)       runId="${2}"; shift;;
    --branch)       branch="${2}"; shift;;
    --repo)         repo="${2}"; shift;;
    --binary-path)  binaryPath="${2}"; shift;;
    *)              echo "Unrecognized argument \"${1}\"."; exit 1;;
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
if [ -z "$binaryPath" ]; then
  # Default to ~/.azure/bin
  binaryPath="$HOME/.azure/bin"
fi
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
gh run download -R $repo $runId -n "bicep-release-$platform-$arch" --dir $tmpDir

# Install
if [[ $platform == "osx" ]]; then
  # Ad-hoc sign the binary
  codesign -s - "$tmpDir/bicep"
fi

mkdir -p $binaryPath
mv "$tmpDir/bicep" "$binaryPath/bicep"
chmod +x "$binaryPath/bicep"

version=$("$binaryPath/bicep" --version | sed 's/^.* \([0-9]*\.[0-9]*\.[0-9]*\) .*/\1/')
echo "Installed Bicep $version from https://github.com/$repo/actions/runs/$runId to $binaryPath/bicep"

# Cleanup
rm -Rf $tmpDir