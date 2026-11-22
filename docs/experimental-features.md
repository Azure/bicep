# Experimental Features

> [!WARNING]
> Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.
>
> Microsoft customer support cannot provide assistance for issues that are caused by the use of an experimental feature, so use at your own risk.

Bicep exposes experimental features as a means of previewing new functionality that hasn't yet been stabilized for general availability. This provides a valuable way for us to release functionality and collect feedback, without affecting existing users, or introducing features that may require breaking changes.

## List of features

The following features can be optionally enabled through your `bicepconfig.json` file. For information on how to enable them, see [Configure your Bicep environment](https://aka.ms/bicep/config).

### `artifactAliasesAndRedirects`

Enables the artifact reference features from [REP 0022: artifact aliases and redirects](https://github.com/Azure/bicep/issues/19883), which are being implemented in stages. Currently:

- `oci:` is another spelling of `br:`, for example `module storage 'oci:contoso.azurecr.io/bicep/modules/storage:1.0.0'`.
- `file:` references a local module or extension, for example `module network 'file:../shared/network.bicep'` or `extension 'file:./extensions/foo.tgz' as foo`. In the `extensions` section of `bicepconfig.json`, a `file:` path is relative to the configuration file.
- `<alias>::` references an artifact through an alias defined in the `artifacts.aliases` section of `bicepconfig.json`. An alias has a `type`:
  - `oci`: a `registry` and an optional `repositoryPrefix`. `company::storage:1.0.0` with `{ "type": "oci", "registry": "contoso.azurecr.io", "repositoryPrefix": "bicep/modules" }` references `oci:contoso.azurecr.io/bicep/modules/storage:1.0.0`. If `repositoryPrefix` is a complete repository, the reference can name only the tag or digest, as in `storage:::1.0.0`.
  - `ts`: a `subscriptionId` and `resourceGroup`. `specs::webapp:v1` references `ts:<subscriptionId>/<resourceGroup>/webapp:v1`.
  - `file`: a `path` relative to the `bicepconfig.json` that declares the alias. `modules::network/vnet.bicep` with `{ "type": "file", "path": "./modules" }` references `modules/network/vnet.bicep` next to that configuration file, from any folder that uses it.

  `mar::` (`mcr.microsoft.com/bicep`) and `avm::` (`mcr.microsoft.com/bicep/avm`) are predefined, for example `module storage 'avm::res/storage/storage-account:0.32.1'`. They can be overridden by `oci` aliases, for example to use a private mirror. Artifact aliases are independent of `moduleAliases`: `br/<alias>:` and `ts/<alias>:` keep using `moduleAliases`, and `<alias>::` only uses `artifacts.aliases`. Scheme names (`br`, `oci`, `ts` and `file`) can't be used as alias names.

### `assertions`

Should be enabled in tandem with `testFramework` experimental feature flag for expected functionality. Allows you to author boolean assertions using the `assert` keyword comparing the actual value of a parameter, variable, or resource name to an expected value. Assert statements can only be written directly within the Bicep file whose resources they reference. For more information, see [Bicep Experimental Test Framework](https://github.com/Azure/bicep/issues/11967).

### `azExtensionConfig`

Enables configuration for the built-in `az` extension. This allows templates to specify a list of Azure resource provider namespaces via the `providers` property, which are registered at the start of a deployment. ARM will trigger a provider registration for each listed namespace on the target subscription.

Example:

```bicep
extension az with {
  providers: [
    'Microsoft.Storage' 
    'Microsoft.Compute'
  ]
}
```

(Note: This feature will not work until the backend service support has been deployed.)

### `deployCommands`

Enables `deploy`, `what-if` and `teardown` command groups, as well as the `with` syntax in a `.bicepparam` file. For more information, see [Using the Deploy Commands](./experimental/deploy-commands.md).

### `legacyFormatter`

Enables code formatting with the legacy formatter. This feature flag is introduced to ensure a safer transition to the v2 formatter that implements a pretty-printing algorithm. It is intended for temporary use and will be phased out soon.

### `localDeploy`

Enables Bicep to run deployments locally, so that you can run Bicep extensions without a dependency on Azure (for example, to run scripts, or to interact with non-Azure APIs like Kubernetes or GitHub). For more information, see [Using Local Deploy](./experimental/local-deploy.md).

### `moduleExtensionConfigs`

Moves defining extension configurations to the module level rather than from within a template. The feature also
includes enhancements for Deployment stacks extensibility integration. This feature is not ready for use.

### `ociEnabled`

Switches all OCI registry operations to the ORAS transport, including Azure Container Registry (ACR), and enables Docker-based authentication for non-Azure registries (e.g. GitHub Container Registry, Docker Hub, self-hosted registries). When disabled (the default), Bicep continues to use the ACR SDK. For more information, see [Using OCI Registries](./experimental/oci-registries.md).

### `patch`

Enables the `@patch()` decorator for deploying resources using the PATCH HTTP method instead of PUT. This feature is restricted to Azure Policy DeployIfNotExists (DINE) scenarios, allowing policies to make incremental changes to existing resources without full redeployment.

> **Note**: This feature is intended for internal Azure Policy scenarios and is not intended for general public use. It requires specific backend support that is only available in Policy-initiated deployments.

```bicep
@patch()
resource existingVm 'Microsoft.Compute/virtualMachines@2023-01-01' = {
  name: 'my-existing-vm'
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_D4s_v3'  // Only this property will be patched
    }
  }
}
```

### `resourceInfoCodegen`

Enables the 'resourceInfo' function for simplified code generation.

### `resourceTypedParamsAndOutputs`

Enables the type for a parameter or output to be of type resource to make it easier to pass resource references between modules. This feature is only partially implemented. See [Simplifying resource referencing](https://github.com/azure/bicep/issues/2245).

### `runtimeValuesInTagsAndSku`

Allows the use of runtime values (such as this.existingResource()) in `tags` and `sku` properties. By default, these properties are flagged as deploy-time constants, meaning they cannot reference runtime resource properties. Enabling this feature relaxes that restriction. (Note: This feature will not work until the backend service support has been deployed)

```bicep
resource example 'Microsoft...' = {
  name: 'example'
  location: resourceGroup().location
  sku: {
    name: this.existingResource().?sku.name ?? 'Standard'
  }
  tags: this.existingResource().?tags ?? {}
}
```

### `sourceMapping`

Enables basic source mapping to map an error location returned in the ARM template layer back to the relevant location in the Bicep file.

### `symbolicNameCodegen`

Allows the ARM template layer to use a new schema to represent resources as an object dictionary rather than an array of objects. This feature improves the semantic equivalence of the Bicep and ARM templates, resulting in more reliable code generation. Enabling this feature has no effect on the Bicep layer's functionality.

### `testFramework`

Should be enabled in tandem with `assertions` experimental feature flag for expected functionality. Allows you to author client-side, offline unit-test test blocks that reference Bicep files and mock deployment parameters in a separate `test.bicep` file using the new `test` keyword. Test blocks can be run with the command *bicep test <filepath_to_file_with_test_blocks>* which runs all `assert` statements in the Bicep files referenced by the test blocks. For more information, see [Bicep Experimental Test Framework](https://github.com/Azure/bicep/issues/11967).
### `userDefinedConstraints`

Enables the `@validate()` decorator on types, type properties, parameters, and outputs. The decorator takes two arguments: 1) a lambda function that accepts the decorator target's value and returns a boolean indicating if the value is valid (`true`) or not (`false`), and 2) an optional error message to use if the lambda returns `false`.

```bicep
@validate(x => startsWith(x, 'foo')) // <-- Accepts 'food' or 'fool' but causes the deployment to fail if 'booed' was supplied
param p string
```

### `waitUntil`

The feature introduces waitUntil decorators on resource data type. waitUntil() decorator waits for the resource until its usable based on the desired property's state. 

## Other experimental functionality

### `publish-extension` CLI Command

Command that allows the publishing of extensions to container registries. For more information, see [Using the Publish Extension Command](./experimental/publish-extension-command.md).

### Bicep MCP Server

See [Using Bicep MCP Server in VS Code (Preview!)](./experimental/mcp-tools.md).

### `docs` CLI Command

Generates module documentation. For command and template model details, see [Generate module documentation](./experimental/docs-commands.md).
