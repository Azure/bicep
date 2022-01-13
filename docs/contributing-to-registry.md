>**Note**: Please ignore the doc for now as it's a work in progress. We are still working on creating the Bicep public registry, and the tool mentioned in the doc has not been published yet.

# Contributing to Bicep public registry

Currently, we only accept contributions from Microsoft employees. The guide was created to help teams within Microsoft with the development of Bicep public registry modules. External customers can propose a new module by opening an [issue]() in the [TBD]() repository.

## Prerequisite
- Create a fork of the [TBD]() repository and clone the fork to your local machine.
- Install [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime).
  <!-- TODO: Add link to Nuget once the tool is published -->
- Install the [Bicep registry module]() .NET tool by running:
  - `dotnet tool install -g brm`

## Creating a new module

### Making a proposal for new module
<!-- TODO: create an issue template in the quickstart repo -->
Before creating a new module, you must fill out this [issue template](https://github.com/Azure/azure-quickstart-templates/issues/new) to make a proposal. Once the proposal is approved, proceed with the following steps. You should not send out a pull request to add a module without an associated approval as the pull request will be rejected.

### Creating a directory for the new module
<!-- TODO: need to discuss the pattern of the module path -->
Add a new directory under the `modules` folder in your local azure-quickstart-templates repository with the path following the pattern `<ResourceProviderName>/<ModuleName>/<ModuleMajorVersion>.<ModuleMinorVersion>`, and the path must be in lowercase. For examples: 
- `microsoft.storage/storageaccounts/0.9`
- `microsoft.web/webapplications/1.0`

### Generating module files
Open a terminal and navigate to the newly created folder. From there, run the following command to generate the required files for the Bicep public registry module:
```
brm generate
```

You should be able to see these files created in the module folder:
- `metadata.json`
  - The module manifest file that contains metadata you must provide, such as `description`.
- `main.bicep`
  - An empty Bicep file to be updated.
- `azuredeploy.json`
  - The ARM template file generated from `main.bicep`. This is the file that will be published to the Bicep public registry. You should not modify this file.
- `azuredeploy.parameters.json`
  - The ARM template parameters file for `azuredeploy.json`. The file is generated for running template deployment validation. You must provide values for the required parameters in the file.
- `README.md`
  - The README file generated based on the contents of `metadata.json` and `main.bicep`. Do not update this file manually.
- `version.json`
  - The version file, together with the `azuredeploy.json` file, are used to calculate the patch version number of the module. Every time `azuredeploy.json` is changed, the patch version number gets bumped. The full version (`<ModuleMajorVersion>.<ModuleMinorVersion>.<ModulePatchVersion>`) will then be assigned to the module when it gets published to the Bicep public module registry. The process is done by the module publishing CI automatically. You should not edit this contents of the file.

### Authoring module files
The only two files that you need to edit are `metadata.json` and  `main.bicep`. When authoring `main.bicep`, make sure to provide a description for each parameter and output. You are free to create other Bicep files and reference them as local modules in `main.bicep` if needed. You can also reference other external modules available in the Bicep public registry to help build your module. Once you are done, run `bicep-registry-module generate` again to refresh the other files. You may need to update `azuredeploy.parameters.json` to provide parameter values after the generation if there are changes to the template parameters.

## Updating an existing module
To update an existing module, simply follow the [Authoring module files](#authoring-module-files) section to update and regenerate the module files.

## Validating a module

> Before running the command, it is recommended to run `generate` and check `azuredeploy.parameters.json` to ensure all files are up-to-date.

You may use the Bicep registry module tool to validate files in a registry module. To do so, invoke the follow command from the module folder:
```
brm validate
```

## Submitting a pull request
When you are done with editing and validating the module files, you can commit your changes and open a pull request in the [TBD]() repository. You must link the new module proposal in the pull request description if you are trying to add a new module.

## Publishing a module
Once your pull request is approved and merged to the main, a CI job will be triggered to check if there are changes made to `azuredeploy.json`. If yes, it will automatically bump the version of the module and publish the module to the Bicep public registry.


