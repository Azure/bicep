# Experimental Features

> [!WARNING]
> Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.

Bicep exposes experimental features as a means of previewing new functionality that hasn't yet been stabilized for general availability. This provides a valuable way for us to release functionality and collect feedback, without affecting existing users, or introducing features that may require breaking changes.

## List of features

The following features can be optionally enabled through your `bicepconfig.json` file. For information on how to enable them, see [Configure your Bicep environment](https://aka.ms/bicep/config).

### `assertions`

Should be enabled in tandem with `testFramework` experimental feature flag for expected functionality. Allows you to author boolean assertions using the `assert` keyword comparing the actual value of a parameter, variable, or resource name to an expected value. Assert statements can only be written directly within the Bicep file whose resources they reference. For more information, see [Bicep Experimental Test Framework](https://github.com/Azure/bicep/issues/11967).

### `deployCommands`

Enables `deploy`, `what-if` and `teardown` command groups, as well as the `with` syntax in a `.bicepparam` file. For more information, see [Using the Deploy Commands](./experimental/deploy-commands.md).

### `desiredStateConfiguration`

Allows you to author configuration documents for [Microsoft's Desired State Configuration platform](https://github.com/PowerShell/DSC) using `targetScope = 'desiredStateConfiguration'`. If enabled, the file must only contain DSC resource instances. The built file is a valid configuration document to be used with the CLI. For example, `dsc.exe config test --file example.json`. This feature is in early development.

### `extendableParamFiles`

Enables the ability to extend bicepparam files from other bicepparam files. For more information, see [Extendable Bicep Params Files](./experimental/extendable-param-files.md).

### `legacyFormatter`

Enables code formatting with the legacy formatter. This feature flag is introduced to ensure a safer transition to the v2 formatter that implements a pretty-printing algorithm. It is intended for temporary use and will be phased out soon.

### `localDeploy`

Enables Bicep to run deployments locally, so that you can run Bicep extensions without a dependency on Azure (for example, to run scripts, or to interact with non-Azure APIs like Kubernetes or GitHub). For more information, see [Using Local Deploy](./experimental/local-deploy.md).

### `moduleExtensionConfigs`

Moves defining extension configurations to the module level rather than from within a template. The feature also
includes enhancements for Deployment stacks extensibility integration. This feature is not ready for use.

### `resourceInfoCodegen`

Enables the 'resourceInfo' function for simplified code generation.

### `resourceTypedParamsAndOutputs`

Enables the type for a parameter or output to be of type resource to make it easier to pass resource references between modules. This feature is only partially implemented. See [Simplifying resource referencing](https://github.com/azure/bicep/issues/2245).

### `sourceMapping`

Enables basic source mapping to map an error location returned in the ARM template layer back to the relevant location in the Bicep file.

### `symbolicNameCodegen`

Allows the ARM template layer to use a new schema to represent resources as an object dictionary rather than an array of objects. This feature improves the semantic equivalence of the Bicep and ARM templates, resulting in more reliable code generation. Enabling this feature has no effect on the Bicep layer's functionality.

### `testFramework`

Should be enabled in tandem with `assertions` experimental feature flag for expected functionality. Allows you to author client-side, offline unit-test test blocks that reference Bicep files and mock deployment parameters in a separate `test.bicep` file using the new `test` keyword. Test blocks can be run with the command *bicep test <filepath_to_file_with_test_blocks>* which runs all `assert` statements in the Bicep files referenced by the test blocks. For more information, see [Bicep Experimental Test Framework](https://github.com/Azure/bicep/issues/11967).
### `thisNamespace`

Enables the `this` namespace for accessing the current resource instance. The `this` namespace is only discoverable in resource bodies. Currently, `this.exists()` and `this.existingResource()` are available for usage. `this.exists()` returns a bool indicating the existence of the current resource. `this.existingResource()` returns null if the resource does not exist and returns the full resource if the resource exists. (Note: This feature will not work until the backend service support has been deployed) 
```
resource usingThis 'Microsoft...' = {
  name: 'example'
  location: 'eastus'
  properties: {
    property1: this.exists() ? 'resource exists' : 'resource does not exist'
    property2: this.existingResource().?properties.?property2
    property3: this.existingResource().?tags
  }
}
```

### `userDefinedConstraints`

Enables the `@validate()` decorator on types, type properties, parameters, and outputs. The decorator takes two arguments: 1) a lambda function that accepts the decorator target's value and returns a boolean indicating if the value is valid (`true`) or not (`false`), and 2) an optional error message to use if the lambda returns `false`.

```bicep
@validate(x => startsWith(x, 'foo')) // <-- Accepts 'food' or 'fool' but causes the deployment to fail if 'booed' was supplied
param p string
```

### `waitAndRetry`

The feature introduces waitUntil and retryOn decorators on resource data type. waitUnitl() decorator waits for the resource until its usable based on the desired property's state. retryOn() will retry the deployment if one if the listed exception codes are encountered.

## Other experimental functionality

### `publish-extension` CLI Command

Command that allows the publishing of extensions to container registries. For more information, see [Using the Publish Extension Command](./experimental/publish-extension-command.md).

### `snapshot` CLI Command

Generate a normalized list of resources to file, which can then be used to generate a visual diff for changes. For more information, see [Using the Snapshot Command](./experimental/snapshot-command.md).

### Bicep MCP Server

See [Using Bicep MCP Server in VS Code (Preview!)](./experimental/mcp-tools.md).
