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
# Verify you can now access the 'bicep' command
bicep --version
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
# Verify you can now access the 'bicep' command
bicep --version
```

### Windows
#### Setting up $PATH (only required on first install)
1. On the **Start** menu, right-click **Computer**.
1. On the context menu, click **Properties**.
1. In the **System** dialog box, click **Advanced system settings**.
1. On the **Advanced** tab of the **System Properties** dialog box, click **Environment Variables**.
1. In the **System Variables** box of the **Environment Variables** dialog box, scroll to **Path** and select it.
1. Click the lower of the two **Edit** buttons in the dialog box.
1. In the **Edit System Variable** dialog box, scroll to the end of the string in the **Variable** value box.
1. Edit the current value to add `;%HOME\.bicep` at the end. Take care to make sure this value is correctly formatted.
1. Click **OK** in three successive dialog boxes, and then close the **System** dialog box.

#### Installing (powershell)
```powershell
# Fetch the latest Bicep CLI binary
Invoke-WebRequest -Uri "https://github.com/Azure/bicep/releases/download/latest/win-x64/bicep.exe" -OutFile ".\bicep.exe"
# Copy to local folder
$installPath = "$env:HOME\.bicep"
New-Item -ItemType Directory -Path $path -Force
Copy-Item  -Path ".\bicep.exe" -Destination $AdminPath -Recurse -force
# Verify you can now access the 'bicep' command
bicep --version
```