![Build](https://github.com/Azure/bicep/workflows/Build/badge.svg) [![codecov](https://codecov.io/gh/Azure/bicep/branch/master/graph/badge.svg)](https://codecov.io/gh/Azure/bicep)

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
* Bicep is currently not covered by [Azure support plans](https://azure.microsoft.com/en-us/support/plans/) as it is still in early development stages. Expect Bicep to be covered by all support plans starting on the 0.3 version.

## FAQ

**Why create a new language instead of using an existing one?**
Bicep is more of a revision to the existing ARM template language rather than an entirely new language. While most of the syntax has been changed, the core functionality of ARM templates and the runtime remains the same. You have the same template functions, same resource declarations, etc. Part of the complexity with ARM Templates is due to the "DSL" being embedded inside of JSON. With Bicep, we are revising the syntax of this DSL and moving it into its own `.bicep` file format. Before going down this path, we closely evaluated using an existing high-level programming language, but ultimately determined that bicep would be easier to learn for our target audience. We are open to other implementations of bicep in other languages.

**Why not focus your energy on Terraform or other third-party IaC offerings?**
Using terraform can be a great choice depending on the requirements of the organization, and if you are happy using terraform there is no reason to switch. At Microsoft, we are actively investing to make sure the terraform on Azure experience is the best it can be.

That being said, there is a huge customer base using ARM templates today because it provides a unique set of capabilities and benefits. We wanted to make the experience for those customers first-class as well, in addition to making it easier to start for Azure focused customers who have not yet transitioned to infra-as-code.

Fundamentally, we believe that configuration languages and tools are always going to be polyglot and different users will prefer different tools for different situations. We want to make sure all of these tools are great on Azure, Bicep is only a part of that effort.

**Is this ready for production use? If not, when will it be ready?**
Not yet. We wanted to get the 0.1 release out quickly and get feedback while we still have an opportunity to make breaking changes. By the end of the year, we plan to ship an 0.3 release which will be at parity with what you can accomplish with ARM templates. At that point, we will start recommending production usage.

**What are you looking for feedback on?**
The language syntax and the tooling. Now is the best time to make breaking changes, so syntax feedback is very appreciated.

**Is this only for Azure?**
Bicep is a DSL focused on deploying end-to-end solutions in Azure. In practice, that usually means working with some non-Azure APIs (i.e. creating kubernetes deployments or users in a database), so we expect to provide some extensibility points. That being said, in the 0.1 release, you can only create Azure resources that are exposed through the ARM API.

**What happens to my existing ARM Template investments?**
One of our goals is to make the transition from ARM Templates to Bicep as easy as possible. We plan to ship a "decompiler", which will convert an ARM template into an equivalent Bicep file. We also will support using a standard ARM template as a Bicep module without converting it to bicep.

## Reference

* [Complete language spec](./docs/spec/bicep.md)
* [@BicepLang](https://twitter.com/BicepLang)
* [ARM Template Reference](https://docs.microsoft.com/azure/templates/)

## Alternatives

Because we are now treating the ARM Template as an IL, we expect and encourage other implementations of IL (ARM Template) generation. We'll keep a running list of alternatives for creating ARM templates that may better fit your use case.

* [Farmer](https://compositionalit.github.io/farmer/) (@isaacabraham) - Generate and deploy ARM Templates on .NET
* [Cloud Maker](https://cloudmaker.ai) (@cloud-maker-ai) - Draw deployable infrastructure diagrams that are converted to ARM templates

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
