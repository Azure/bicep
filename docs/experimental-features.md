# Experimental Features

> [!WARNING]
> Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.

Bicep exposes experimental features as a means of previewing new functionality that hasn't yet been stabilized for general availability. This provides a valuable way for us to release functionality and collect feedback, without affecting existing users, or introducing features that may require breaking changes.

## List of features
The following features can be optionally enabled through your `bicepconfig.json` file. For information on how to enable them, see [Configure your Bicep environment](https://aka.ms/bicep/config).

### `assertions`
Should be enabled in tandem with `testFramework` experimental feature flag for expected functionality. Allows you to author boolean assertions using the `assert` keyword comparing the actual value of a parameter, variable, or resource name to an expected value. Assert statements can only be written directly within the Bicep file whose resources they reference. For more information, see [Bicep Experimental Test Framework](https://github.com/Azure/bicep/issues/11967).

### `extensibility`
Allows Bicep to use a provider model to deploy non-ARM resources. Currently, we only support a Kubernetes provider. See [Bicep extensibility Kubernetes provider](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-extensibility-kubernetes-provider).

### `sourceMapping`
Enables basic source mapping to map an error location returned in the ARM template layer back to the relevant location in the Bicep file.

### `resourceTypedParamsAndOutputs`
Enables the type for a parameter or output to be of type resource to make it easier to pass resource references between modules. This feature is only partially implemented. See [Simplifying resource referencing](https://github.com/azure/bicep/issues/2245).

### `symbolicNameCodegen`
Allows the ARM template layer to use a new schema to represent resources as an object dictionary rather than an array of objects. This feature improves the semantic equivalence of the Bicep and ARM templates, resulting in more reliable code generation. Enabling this feature has no effect on the Bicep layer's functionality.

### `testFramework`
Should be enabled in tandem with `assertions` experimental feature flag for expected functionality. Allows you to author client-side, offline unit-test test blocks that reference Bicep files and mock deployment parameters in a separate `test.bicep` file using the new `test` keyword. Test blocks can be run with the command *bicep test <filepath_to_file_with_test_blocks>* which runs all `assert` statements in the Bicep files referenced by the test blocks. For more information, see [Bicep Experimental Test Framework](https://github.com/Azure/bicep/issues/11967).

### `userDefinedFunctions`
Allows you to define your own custom functions. See [User-defined functions in Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/user-defined-functions).

### `optionalModuleNames`
Enabling this feature makes the `name` property in the body of `module` declarations optional. When a `module` omits the `name` property with the feature enabled, the Bicep compiler will automatically generate an expression for the name of the resulting nested deployment in the JSON. If you specify the `name` property, the compiler will use the specified expression in the resulting JSON. For more information, see [Added optional module names as an experimental feature](https://github.com/Azure/bicep/pull/12600).

### `resourceDerivedTypes`
If enabled, templates can reuse resource types wherever a type is expected. For example, to declare a parameter `foo` that should be usable as the name of an Azure Storage account, the following syntax would be used: `param foo resouce<'Microsoft.Storage/storageAccounts@2022-09-01'>.name`. **NB:** Because resource types may be inaccurate in some cases, no constraints other than the ARM type primitive will be enforced on resource derived types within the ARM deployment engine. Resource-derived types will be checked by Bicep at compile time, but violations will be emitted as warnings rather than errors.

## Other experimental functionality

### `publish-provider` CLI Command
Command that allows the publishing of providers to container registries. For more information, see [Using the Publish Provider Command](./experimental/publish-provider-command.md).

### `jsonrpc` CLI Command
Command that launches the CLI in a JSONRPC server mode. Useful for invoking the CLI programatically to consume structured output, and to avoid cold-start delays when compiling multiple files. For more information, see [Using the JSONRPC Command](./experimental/jsonrpc-command.md).

### Deployment Pane
The Deployment Pane is a UI panel in VSCode that allows you to connect to your Azure subscription and execute validate, deploy & whatif operations and get instant feedback without leaving the editor. For more information, see [Using the Deployment Pane](./experimental/deploy-ui.md).
