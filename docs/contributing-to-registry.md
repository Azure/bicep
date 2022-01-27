> **Note**: The doc it's a work in progress and is subject to change. We are still creating the Bicep public registry, and the tool mentioned in the doc has not been published yet.

# Contributing to Bicep public registry

Currently, we only accept contributions from Microsoft employees. The guide was created to help teams within Microsoft with the development of Bicep public registry modules. External customers can propose a new module by opening an [issue](https://github.com/Azure/bicep-registry-modules/issues) in the [Azure/bicep-registry-modules](https://github.com/Azure/bicep-registry-modules) repository.

## Prerequisite

- Create a fork of the [Azure/bicep-registry-modules](https://github.com/Azure/bicep-registry-modules) repository and clone the fork to your local machine.
- Install [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)
  <!-- TODO: Add link to Nuget once the tool is published -->
- Install the [Bicep registry module]() .NET tool by running:
  - `dotnet tool install -g brm`

## Creating a new module

### Making a proposal for new module

<!-- TODO: create an issue template in the registry modules repo -->

Before creating a new module, you must fill out this [issue template](https://github.com/Azure/bicep-registry-modules/issues/new) to make a proposal. Once the proposal is approved, proceed with the following steps. You should not send out a pull request to add a module without an associated approval as the pull request will be rejected.

### Creating a directory for the new module

<!-- TODO: need to discuss the pattern of the module path -->

Add a new directory under the `modules` folder in your local bicep-registry-modules repository with the path in lowercase following the pattern `<ResourceProviderName>/<ModuleName>/<ModuleMajorVersion>.<ModuleMinorVersion>`. Note that `<ModuleName>` does not have to be a resource type name, since a module may consist of multiple resource types. If you need to create a child module, place it inside a nested folder under the parent module's folder. For examples:

- `microsoft.web/containerized-web-apps/1.0`
- `microsoft.web/containerized-web-apps/configs/1.0`
- `microsoft.compute/virtual-machines-with-public-ip-addresses/1.1`

### Generating module files

Open a terminal and navigate to the newly created folder. From there, run the following command to generate the required files for the Bicep public registry module:

```
brm generate
```

You should be able to see these files created in the module folder:
| File Name | Description |
| :--------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `metadata.json` | An JSON file containing module metadata. You must edit the file to provide the metadata values. |
| `main.bicep` | An empty Bicep file that you need to update. This is the main module file. |
| `test/main.test.bicep` | A Bicep file to be deployed in the PR merge validation pipeline to test if `main.bicep` is deployable. You must add at least one test to the file. A module referencing `main.bicep` is considered a test. |
| `main.json` | The main ARM template file compiled from `main.bicep`. This is the artifact that will be published to the Bicep public registry. You should not modify the file. |
| `README.md` | The README file generated based on the contents of `metadata.json` and `main.bicep`. Do not update this file manually. |
| `version.json` | The module version file. It is used together with `main.json` to calculate the patch version number of the module. Every time `main.json` is changed, the patch version number gets bumped. The full version (`<ModuleMajorVersion>.<ModuleMinorVersion>.<ModulePatchVersion>`) will then be assigned to the module before it gets published to the Bicep public module registry. The process is handled by the module publishing CI automatically. You should not edit the contents of this file. |

### Authoring module files

The only files that you need to edit are `metadata.json`, `main.bicep`, and `test/main.test.bicep`.

The `metadata.json` file contains metadata of the module including `name`, `description`, and `owner`. You must provide the values for them. Below is a sample metadata file:

```JSONC
{
  "$schema": "https://aka.ms/bicep-registry-module-metadata-schema#",
  // The name of the module (10 - 60 characters).
  "name": "Sample module",
  // The description of the module (10 - 1000 characters).
  "description": "Sample module description",
  // The owner of the module. Must be a GitHub username or a team under the Azure organization
  "owner": "sampleusername"
}

```

The `main.bicep` file is the public interface of the module. When authoring `main.bicep`, make sure to provide a description for each parameter and output. You are free to create other Bicep files inside the module folder and reference them as local modules in `main.bicep` if needed. You may also reference other registry modules to help build your module. If you do so, make sure to add them as external references with specific version numbers. You should not reference other registry modules through local file path, since they may get updated overtime.

<!-- TODO: add parameter generation guideline -->

The `test/main.test.bicep` file is the test file for `main.bicep`. It will be deployed to a test environment in the PR merge pipeline to make sure `main.bicep` is deployable. You must add at least one test to the file. To add a test, simply create a module referencing `main.bicep` and provide values for the required parameters. You may write multiple tests to ensure different paths of the module are covered.

Once you are done editing the files, run `brm generate` again to refresh the other files.

## Updating an existing module

To update an existing module, refer to the [Authoring module files](#authoring-module-files) section to update and regenerate the module files. If you need to update the major or minor version, change the version in the module version folder name before the regeneration so that the version file can get updated.

## Validating a module

> Before running the command, don't forget to run `generate` and check `test/main.test.bicep` to ensure all files are up-to-date.

You may use the Bicep registry module tool to validate the contents of the registry module files. To do so, invoke the follow command from the module folder:

```
brm validate
```

## Submitting a pull request

> The `brm validate` command does not deploy `test/main.test.bicep`, but it is highly recommended that you run a test deployment locally using Azure CLI or Azure PowerShell before submitting a pull request.

Once the module files are validated locally, you can commit your changes and open a pull request in the [Azure/bicep-registry-modules](https://github.com/Azure/bicep-registry-modules) repository. You must link the new module proposal in the pull request description if you are trying to add a new module.

## Publishing a module

Once your pull request is approved and merged to the main, a CI job will be triggered to check if there are changes made to `main.json`. If yes, it will automatically bump the version of the module and publish the module to the Bicep public registry.
