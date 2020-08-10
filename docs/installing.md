# Installation Instructions

## Bicep VSCode Extension

### Manually
Download the latest extension from [here](https://github.com/Azure/bicep/files/latest/bicep.vsix).

In the VSCode Extensions view, use 'Install from VSIX' and provide the path to the downloaded VSIX file.

### Via command line
#### Fetch the latest Bicep VSCode extension
```sh
curl -LO https://github.com/Azure/bicep/releases/download/latest/vscode-bicep.vsix
```

#### Install the extension
```sh
code --install-extension vscode-bicep.vsix
```

## Bicep CLI

### Linux

#### Fetch the latest Bicep CLI binary
```sh
curl -LO https://github.com/Azure/bicep/releases/download/latest/linux-x64/bicep
```

#### Mark it as executable
```sh
chmod +x ./bicep
```

#### Add bicep to your PATH
```sh
sudo mv ./bicep /usr/local/bin/bicep
```

### macOS

#### Fetch the latest Bicep CLI binary
```sh
curl -LO https://github.com/Azure/bicep/releases/download/latest/osx-x64/bicep
```

#### Mark it as executable
```sh
chmod +x ./bicep
```

#### Add Gatekeeper exception
```sh
sudo spctl --add ./bicep
```

#### Add bicep to your PATH
```sh
sudo mv ./bicep /usr/local/bin/bicep
```