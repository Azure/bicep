# Setup your bicep development environment

To get the best bicep authoring experience, you will need two components:
  
* Bicep CLI (required) - Compiles bicep files into ARM templates. Cross-platform.
* Bicep VS Code Extension - Authoring support, intellisense, validation. Optional, but recommended.

## Install the Bicep VSCode Extension

### Manually
Download the latest extension by clicking [here](https://github.com/Azure/bicep/releases/download/latest/vscode-bicep.vsix).

>NOTE: Do **not** double-click the `vscode-bicep.vsix` file

Open VSCode, and in the Extensions view, select 'Install from VSIX'. Provide the path to the VSIX file you downloaded.

### Via command line
```sh
# Fetch the latest Bicep VSCode extension
curl -Lo vscode-bicep.vsix https://github.com/Azure/bicep/releases/download/latest/vscode-bicep.vsix
# Install the extension
code --install-extension vscode-bicep.vsix
```

## Verify the Bicep VS Code extension (Language service)

Open the `main.bicep` file in VS code. If the extension is installed, you should see syntax highlighting working, and you should see the language in the lower right hand corner of the VS code window change to `bicep`.

## Bicep CLI

### Linux
```sh
# Fetch the latest Bicep CLI binary
curl -Lo bicep https://github.com/Azure/bicep/releases/download/latest/bicep-linux-x64
# Mark it as executable
chmod +x ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
# Verify you can now access the 'bicep' command
bicep --version
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
bicep --version
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
bicep --version
```

### Validate the install

Validate that the cli is running by creating a blank file `main.bicep` and then running:

```bash
bicep build main.bicep
```

You should get an output json file of the same name in your current directory -- in this case `main.json`. It should be a skeleton ARM JSON template:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {},
  "functions": [],
  "variables": {},
  "resources": [],
  "outputs": {}
}
```

## Next steps

Now that you have the tooling installed, you can start the tutorial which will teach you full bicep capabilities:

[1 - Working with a basic bicep file](./01-simple-template.md)
