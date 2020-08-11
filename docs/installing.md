# Installation Instructions

## Bicep VSCode Extension

### Manually
Download the latest extension by clicking [here](https://github.com/Azure/bicep/files/latest/bicep.vsix).

Open VSCode, and in the Extensions view, select 'Install from VSIX'. Provide the path to the VSIX file you downloaded.

### Via command line
```sh
# Fetch the latest Bicep VSCode extension
curl -LO https://github.com/Azure/bicep/releases/download/latest/vscode-bicep.vsix
# Install the extension
code --install-extension vscode-bicep.vsix
```

## Bicep CLI

### Linux
```sh
# Fetch the latest Bicep CLI binary
curl -LO https://github.com/Azure/bicep/releases/download/latest/linux-x64/bicep
# Mark it as executable
chmod +x ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
```

### macOS
```sh
# Fetch the latest Bicep CLI binary
curl -LO https://github.com/Azure/bicep/releases/download/latest/osx-x64/bicep
# Mark it as executable
chmod +x ./bicep
# Add Gatekeeper exception (requires admin)
sudo spctl --add ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
```