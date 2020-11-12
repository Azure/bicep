# Setup your bicep development environment

To get the best bicep authoring experience, you will need two components:

* Bicep CLI (required) - Compiles bicep files into ARM templates. Cross-platform.
* Bicep VS Code Extension - Authoring support, intellisense, validation. Optional, but recommended.

## Install the Bicep CLI

### Linux
```sh
# Fetch the latest Bicep CLI binary
curl -Lo bicep https://github.com/Azure/bicep/releases/latest/download/bicep-linux-x64
# Mark it as executable
chmod +x ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
# Verify you can now access the 'bicep' command
bicep --help
# Done!
  
```

### macOS

#### Installing via homebrew

```sh
# Add the tap for bicep
brew tap azure/bicep https://github.com/azure/bicep

# Install the tool
brew install azure/bicep/bicep
```

#### Manual install

```sh
# Fetch the latest Bicep CLI binary
curl -Lo bicep https://github.com/Azure/bicep/releases/latest/download/bicep-osx-x64
# Mark it as executable
chmod +x ./bicep
# Add Gatekeeper exception (requires admin)
sudo spctl --add ./bicep
# Add bicep to your PATH (requires admin)
sudo mv ./bicep /usr/local/bin/bicep
# Verify you can now access the 'bicep' command
bicep --help
# Done!
  
```

### Windows

#### Installer
* Download the [latest Windows installer](https://github.com/Azure/bicep/releases/latest/download/bicep-setup-win-x64.exe).
* Run the downloaded executable. The installer does not require administrative privileges.
* Step through the installation wizard.
* After the installation, Bicep CLI will be added to your user PATH. If you have any command shell windows open (`cmd`, `PowerShell`, or similar), you will need to close and reopen them for the PATH change to take effect.

#### PowerShell
```powershell
# Create the install folder
$installPath = "$env:USERPROFILE\.bicep"
$installDir = New-Item -ItemType Directory -Path $installPath -Force
$installDir.Attributes += 'Hidden'
# Fetch the latest Bicep CLI binary
(New-Object Net.WebClient).DownloadFile("https://github.com/Azure/bicep/releases/latest/download/bicep-win-x64.exe", "$installPath\bicep.exe")
# Add bicep to your PATH
$currentPath = (Get-Item -path "HKCU:\Environment" ).GetValue('Path', '', 'DoNotExpandEnvironmentNames')
if (-not $currentPath.Contains("%USERPROFILE%\.bicep")) { setx PATH ($currentPath + ";%USERPROFILE%\.bicep") }
if (-not $env:path.Contains($installPath)) { $env:path += ";$installPath" }
# Verify you can now access the 'bicep' command.
bicep --help
# Done!
  
```

## Install the Bicep VS Code extension

### Installing inside VS code
* Open VS Code
* In the Extensions tab, search for "Bicep".
* Click "Install"

>**Note**: The Bicep VS code extension versions older than 0.2 must be uninstalled before or after the installation of the new version. Otherwise, both extension versions will run side by side and you will see duplicated and/or inconsistent errors. Versions 0.2 or newer do not require uninstallation and will upgrade correctly.

### Installing via the Visual Studio Marketplace
* Open your brownser and navigate to https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-bicep
* Click "Install" which should open VS code extension installation page
* Click "Install" in VS Code.

### Verify the Bicep VS Code extension is running

Open a file called `main.bicep` VS code. If the extension is installed, you should see syntax highlighting working, and you should see the `language mode` in the lower right hand corner of the VS code window change to `bicep`.

## Next steps

Now that you have the tooling installed, you can start the tutorial which will teach you full bicep capabilities:

[1 - Working with a basic bicep file](./tutorial/01-simple-template.md)

### Install the "nightly" builds of bicep

If you'd like to try the latest pre-release bits of bicep before they are released, you can [follow instructions for installing the nightly builds](./installing-nightly.md). Note, these builds are much more likely to have known or unknown bugs.
