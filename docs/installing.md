# Setup your bicep development environment

To get the best bicep authoring experience, you will need two components:
  
* Bicep CLI (required) - Compiles bicep files into ARM templates. Cross-platform.
* Bicep VS Code Extension - Authoring support, intellisense, validation. Optional, but recommended.

## Install the Bicep CLI

### Linux
```sh
# Fetch the latest Bicep CLI binary
curl -Lo bicep https://github.com/Azure/bicep/releases/download/latest/bicep-linux-x64
# Mark it as executable
chmod +x ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
# Verify you can now access the 'bicep' command
bicep --help
```

### macOS
```sh
# Fetch the latest Bicep CLI binary
curl -Lo bicep https://github.com/Azure/bicep/releases/download/latest/bicep-osx-x64
# Mark it as executable
chmod +x ./bicep
# Add Gatekeeper exception (requires admin)
sudo spctl --add ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
# Verify you can now access the 'bicep' command
bicep --help
```

### Windows
#### Setting up $PATH (only required on first install)
1. Open the start menu, and search for **env**
1. Select the item named **Edit the system environmental variables**
1. In the window that appears, press **Environmental Variables...**
1. Under the **User variables** section, find the variable named **Path**, and press **Edit...**
1. In the edit window that appears, press **New**
1. Enter `%USERPROFILE%\.bicep` for the value, and press **OK**
1. Press **OK** to close the other 2 windows that were opened

#### Installing (PowerShell)
```powershell
# Create the install folder
$installPath = "$env:USERPROFILE\.bicep"
$installDir = New-Item -ItemType Directory -Path $installPath -Force
$installDir.Attributes += 'Hidden'
# Fetch the latest Bicep CLI binary
(New-Object Net.WebClient).DownloadFile("https://github.com/Azure/bicep/releases/download/latest/bicep-win-x64.exe", "$installPath\bicep.exe")
# Verify you can now access the 'bicep' command
bicep --help
```

## Install the Bicep VS Code Extension

### Manually
Download the latest extension by clicking [here](https://github.com/Azure/bicep/releases/download/latest/vscode-bicep.vsix).

>NOTE: You cannot install the `vscode-bicep.vsix` file by opening it directly.

Open VS Code, and in the Extensions view, select 'Install from VSIX'. Provide the path to the VSIX file you downloaded.

### Via command line
```sh
# Fetch the latest Bicep VS Code extension
curl -Lo vscode-bicep.vsix https://github.com/Azure/bicep/releases/download/latest/vscode-bicep.vsix
# Install the extension
code --install-extension vscode-bicep.vsix
```

### Verify the Bicep VS Code extension is running

Open a file called `main.bicep` VS code. If the extension is installed, you should see syntax highlighting working, and you should see the `language mode` in the lower right hand corner of the VS code window change to `bicep`.

## Next steps

Now that you have the tooling installed, you can start the tutorial which will teach you full bicep capabilities:

[1 - Working with a basic bicep file](./01-simple-template.md)
