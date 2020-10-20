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

### Manually
* Download the [latest version of the extension](https://github.com/Azure/bicep/releases/latest/download/vscode-bicep.vsix). **NOTE**: You cannot install the vscode-bicep.vsix file by double-clicking it.
* Open VSCode, and in the Extensions tab, select the options (...) menu in the top right corner and select 'Install from VSIX'. Provide the path to the VSIX file you downloaded.

### Via command line (Linux / macOS)
```sh
# Fetch the latest Bicep VSCode extension
curl -Lo vscode-bicep.vsix https://github.com/Azure/bicep/releases/latest/download/vscode-bicep.vsix
# Install the extension
code --install-extension vscode-bicep.vsix
# Clean up the file
rm vscode-bicep.vsix
# Done!
  
```

### Via command line (Windows PowerShell)
```powershell
# Fetch the latest Bicep VSCode extension
$vsixPath = "$env:TEMP\vscode-bicep.vsix"
(New-Object Net.WebClient).DownloadFile("https://github.com/Azure/bicep/releases/latest/download/vscode-bicep.vsix", $vsixPath)
# Install the extension
code --install-extension $vsixPath
# Clean up the file
Remove-Item $vsixPath
# Done!
  
```

### Verify the Bicep VS Code extension is running

Open a file called `main.bicep` VS code. If the extension is installed, you should see syntax highlighting working, and you should see the `language mode` in the lower right hand corner of the VS code window change to `bicep`.

## Next steps

Now that you have the tooling installed, you can start the tutorial which will teach you full bicep capabilities:

[1 - Working with a basic bicep file](./tutorial/01-simple-template.md)


<br>
<br>
<br>
<br>
<br>

## Installing the "Nightly" build of bicep CLI and VS Code extension

>**Note**: only install the nightly if you'd like to try the bleeding edge capabilities of bicep. These are much more likely to have undiscovered bugs or other issues. 

We are not currently publishing "nightly" releases, but you can grab the latest bits by viewing the latest Action workflows for the `main` branch (or any other branch).

The easiest way to get these artifacts is through the GitHub site. Follow [this link](https://github.com/Azure/bicep/actions) to view the latest Action workflows. Find the most recent build on the `main` branch and select it:

![](./images/bicep-select-action.png)

On the details page, select the artifact you would like to download.

The VSCode extension (`vscode-bicep.vsix`) must be unzipped and then can be installed inside of VS Code or with the `code` CLI.

The CLI (`bicep-release-*-x64`) should replace any current bicep executable that has already been added to your PATH. If you are on Windows and previously installed using the installer (`bicep-setup-win-x64`), then downloading and running the new installer will replace the currently installed version of bicep.
