# Installing the "Nightly" build of Bicep CLI and VS Code extension

> **Note**: only install the nightly if you'd like to try the bleeding edge capabilities of Bicep. These are much more likely to have undiscovered bugs or other issues. If you find anything, please open an issue.

## Via Script
> **Note**: These scripts require the [GitHub CLI](https://cli.github.com/) to have already been installed.

### VSCode Extension
This will install the latest nightly release of the VSCode extension

1. (Mac/Linux) Run the following:
   ```sh
   bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh)
   ```
1. (Windows) Run the following in a PowerShell window:
   ```powershell
   iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) }"
   ```
1. Reload your VSCode window and the nightly Bicep extension will be loaded.

### Azure CLI
This will install the latest nightly Bicep CLI binary to `~/.azure/bin/bicep`, so that it will be automatically picked up by Azure CLI

1. (Mac/Linux) Run the following:
   ```sh
   bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh)
   ```
1. (Windows) Run the following in a PowerShell window:
   ```powershell
   iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) }"
   ```
1. Your Azure CLI install should now be referencing the latest nightly Bicep CLI release.

## Manual
We are not currently publishing "nightly" releases, but you can grab the latest bits by viewing the latest Action workflows for the `main` branch (or any other branch).

The easiest way to get these artifacts is through the GitHub site. Follow [this link](https://github.com/Azure/bicep/actions/workflows/build.yml?query=branch%3Amain+is%3Asuccess) to view the latest successful Action workflows for the `main` branch. Select it to show the related artifacts:

![](./images/bicep-select-action.PNG)

On the details page, select the artifact you would like to download.

![](./images/bicep-select-artifact.png)

The VSCode extension (`vscode-bicep.vsix`) must be unzipped and then can be installed inside of VS Code or with the `code` CLI. To install the extension, execute the following steps:
- Open VSCode.
- In the Extensions tab, select the options (...) menu in the top right corner and select 'Install from VSIX'. Provide the path to the VSIX file you downloaded.
- Click "Install".

The CLI (`bicep-release-*-x64`) should replace any current Bicep executable that has already been added to your PATH. If you are on Windows and previously installed using the installer (`bicep-setup-win-x64`), then downloading and running the new installer will replace the currently installed version of Bicep.

## Advanced Script Options
### VSCode Extension
- Mac/Linux
   ```sh
   # install from a fork repo
   bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --repo anthony-c-martin/bicep

   # install from a custom branch
   bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --branch jeskew/variable-imports

   # install from a specific github action run
   bash <(curl -Ls https://aka.ms/bicep/nightly-vsix.sh) --run-id 6146657618
   ```
- Windows
   ```powershell
   # install from a fork repo
   iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -Repo anthony-c-martin/bicep"

   # install from a custom branch
   iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -Branch jeskew/variable-imports"

   # install from a specific github action run
   iex "& { $(irm https://aka.ms/bicep/nightly-vsix.ps1) } -RunId 6146657618"
   ```

### Bicep CLI
- Mac/Linux
   ```sh
   # install to a custom directory
   bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --binary-path /usr/local/bin

   # install from a fork repo
   bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --repo anthony-c-martin/bicep

   # install from a custom branch
   bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --branch jeskew/variable-imports

   # install from a specific github action run
   bash <(curl -Ls https://aka.ms/bicep/nightly-cli.sh) --run-id 6146657618
   ```
- Windows
   ```powershell
   # install to a custom directory
   iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -BinaryPath C:\"

   # install from a fork repo
   iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Repo anthony-c-martin/bicep"

   # install from a custom branch
   iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -Branch jeskew/variable-imports"

   # install from a specific github action run
   iex "& { $(irm https://aka.ms/bicep/nightly-cli.ps1) } -RunId 6146657618"
   ```