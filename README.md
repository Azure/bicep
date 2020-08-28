![Build](https://github.com/Azure/bicep/workflows/Build/badge.svg)

# Project Bicep - an ARM DSL

***Note:** Bicep is currently an **experimental language** and we expect to ship breaking changes in future releases. It is not yet recommended for production usage. Please take a look at the [known limitations](#known-limitations) prior to opening any issues.*

## What is Bicep?

Bicep is a Domain Specific Language (DSL) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code re-use. Bicep is a **transparent abstraction** over ARM and ARM templates, which means anything that can be done in an ARM Template can be done in bicep (outside of temporary [known limitations](#known-limitations)). All resource `types`, `apiVersions`, and `properties` that are valid in an ARM template are equally valid in Bicep on day one.

Bicep compiles down to standard ARM Template JSON files, which means the ARM JSON is effectively being treated as an Intermediate Language (IL).

## Goals

1. Build the best possible language for describing, validating, and deploying infrastructure to Azure.
1. The language should provide a *transparent abstraction* for the underlying platform. There must be no "onboarding step" to enable it to a new resource `type` and/or `apiVersion` in Bicep.
1. Code should be easy to understand at a glance and straightforward to learn, regardless of your experience with other programming languages.
1. Users should be given a lot of freedom to modularize and reuse their code. Reusing code should not require any 'copy/paste'.
1. Tooling should provide a high level of resource discoverability and validation, and should be developed alongside the compiler rather than added at the end.
1. Users should have a high level of confidence that their code is 'syntactically valid' before deploying.

### Non-goals

1. Build a general purpose language to meet any need. This will not replace general purpose languages and you may still need to do pre or post bicep tasks in a script or high-level programming language.
1. Provide a first-class provider model for non-Azure related tasks. While we will likely introduce an extensibility model at some point, any extension points are intended to be focused on Azure infra or application deployment related tasks.

## Get started with Bicep

To get going with bicep, start by [installing the tooling](./docs/installing.md).

Once the tooling is installed, you can start the [bicep tutorial](./docs/tutorial/01-simple-template.md), which walks you through the structure and capabilities of bicep, deploying bicep files, and converting an ARM template into the equivalent bicep file.

Alternatively, you can try the [Bicep Playground](https://aka.ms/bicepdemo).

## How does Bicep work?

First, author your Bicep code using the Bicep language service as part of the [Bicep VS Code extension](./docs/installing.md#bicep-vs-code-extension), then compile that code into an ARM template using the [Bicep CLI](./docs/installing.md#bicep-cli):

```bash
bicep build ./main.bicep
```

You should see the compiled template `main.json` get created in the same directory as `main.bicep`. Now that you have the ARM Template, you can use all existing ARM Template tooling such as [what-if](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-deploy-what-if?tabs=azure-powershell), the [ARM Tools Extension](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools), and the deployment CLI commands.

You can now deploy this template via any method you would like such as:

```bash
az deployment group create -f ./main.json -g my-rg
```

## How is life better with Bicep?

* Simpler syntax when compared to equivalent JSON
  * No special `[...]` expressions syntax required
    * Directly call parameters or variables in expressions without a function (no more need for `parameters('myParam')`)
  * No quotes on property names (e.g. `"location"`)
  * Simple string interpolation: `'${namePrefix}-vm'` instead of `concat(parameters('namePrefix'), '-vm')`
  * Simpler resource declaration using positional properties to avoid typing common property names like `type` and `apiVersion` explicitly.
  * Direct property access of a resource (e.g. `aks.properties.fqdn` instead of `reference(parameters('aksName')).properties.fqdn`)
* Better copy/paste experience via flexible declaration of types. Different types (e.g. `variables`, `resources`, `outputs`) can be declared anywhere.
  * Previously all parameters had to be declared together in one `"parameters": {}` object, variables had to be declared together in one `"variables": {}` object, etc.
* Automatic dependency management in certain scenarios. Bicep will automatically add `dependsOn` in the compiled ARM Template if the symbolic name is used in another resouce declaration.

## Known limitations

* No support for the `copy` or `condition` property [[#185](https://github.com/Azure/bicep/issues/185), [#186](https://github.com/Azure/bicep/issues/186)]
* No explicit support for deployments across scopes (though this can be done by using the `Microsoft.Resources/deployments` resource and using the `templateLink` or `template` property to insert the full ARM template) [[#187](https://github.com/Azure/bicep/issues/187)]
  * Bicep assumes you are deploying to a resource group, though the generated template can be deployed to any scope
* Single line object and arrays (i.e. `['a', 'b', 'c']`) are not yet supported
* You still need to deploy the compiled template yourself, though we plan to build native support for bicep into the powershell `Az` deployment cmdlets and `az cli` deployment commands
* No IntelliSense whatsoever [[#269](https://github.com/Azure/bicep/issues/269)]
* Minimal resource schema validation. Other than basic validations like correct resource `type` structure and requiring a `name`, you will not get errors for missing or incorrect properties in a resource declaration
* No support for string interpolation in property names [[#267](https://github.com/Azure/bicep/issues/267)]
  * From what we know, this only affects using managed identities for resources. You can still include a hardcoded managed identity resource ID (i.e. `'/subscriptions/.../resourceGroups/.../providers/Microsoft.ManagedIdentity/...': {}`)

## Reference

* [Complete language spec](./docs/spec/bicep.md)
* [@BicepLang](https://twitter.com/BicepLang)
* [ARM Template Reference](https://docs.microsoft.com/azure/templates/)

## Alternatives

Because we are now treating the ARM Template as an IL, we expect and encourage other implementations of IL (ARM Template) generation. We'll keep a running list of alternatives for creating ARM templates that may better fit your use case.

* [Farmer (@isaacabraham)](https://compositionalit.github.io/farmer/) - Generate and deploy ARM Templates on .NET

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
