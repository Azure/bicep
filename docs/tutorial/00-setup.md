# Setup your bicep development environment

## Install the CLI

To write and compile bicep files, you will need the cross-platform **bicep cli**. You can download the latest cli from the [releases]() page and downloading the relevant `Bicep_Release_*` file.

*NOTE: by release, should be able to download an installer to setup path correctly*

Validate that the cli is running by creating a blank file `main.arm` and then running:

```bash
bicep build main.arm
```

You should get an output json file of the same name, in this case `main.json`. It should be a skeleton ARM JSON template.

## Install the Bicep VS Code extension (Language service)

In order to get the best authoring experience, you should install the VS code extension. You can download it by navigating to the [releases]() page and downloading the `vscode-bicep.vsix` file.

Install the extension by opening VS Code and navigating to `View` -> `Command Palette...` and searching for `Extension: install from VSIX...` and selecting the unzipped vsix file. You may be prompted to reload the vs code window. 

Confirm that the extension is working by opening the `main.arm` (or any `.arm`) file. You should see the language in the lower right hand corner changes to `bicep`.

