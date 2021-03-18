![Build](https://github.com/Azure/bicep/workflows/Build/badge.svg) [![codecov](https://codecov.io/gh/Azure/bicep/branch/main/graph/badge.svg)](https://codecov.io/gh/Azure/bicep)

# Project Bicep - an ARM DSL

## What is Bicep?

Bicep is a Domain Specific Language (DSL) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax, improved type safety, and better support for modularity and code re-use. Bicep is a **transparent abstraction** over ARM and ARM templates, which means anything that can be done in an ARM Template can be done in Bicep (outside of temporary [known limitations](#known-limitations)). All resource `types`, `apiVersions`, and `properties` that are valid in an ARM template are equally valid in Bicep on day one.

Bicep code is transpiled to standard ARM Template JSON files, which effectively treats the ARM Template as an Intermediate Language (IL).

[![Video overview of Bicep](http://img.youtube.com/vi/l85qv_1N2_A/0.jpg)](http://www.youtube.com/watch?v=l85qv_1N2_A "Azure Bicep March 2021: Learn everything about the next generation of ARM Templates")

## Goals

1. Build the best possible language for describing, validating, and deploying infrastructure to Azure.
1. The language should provide a *transparent abstraction* for the underlying platform. There must be no "onboarding step" to enable Bicep support for a new resource type and/or api version.
1. Code should be easy to understand at a glance and straightforward to learn, regardless of your experience with other programming languages.
1. Users should be given a lot of freedom to modularize and re-use their code. Code re-use should not require any 'copy/paste'-ing.
1. Tooling should provide a high level of resource discoverability and validation, and should be developed alongside the compiler rather than added at the end.
1. Users should have a high level of confidence that their code is 'syntactically valid' before deploying.

### Non-goals

1. Build a general purpose language to meet any need. This will not replace general purpose languages and you may still need to do pre or post-Bicep execution tasks in a script or high-level programming language.
1. Provide a first-class provider model for non-Azure related tasks. While we will likely introduce an extensibility model at some point, any extension points are intended to be focused on Azure infra or application deployment related tasks.

## Get started with Bicep

To get going with Bicep:

1. **Start by [installing the tooling](./docs/installing.md).**
1. **Complete the [Bicep tutorial](./docs/tutorial/01-simple-template.md)**
  * walks you through the structure and capabilities of Bicep, deploying Bicep files, and converting an ARM template into the equivalent Bicep file.

Alternatively, you can try the [Bicep Playground](https://aka.ms/bicepdemo) or use the [VSCode Devcontainer/Codespaces](https://github.com/Azure/vscode-remote-try-bicep) repo to get a preconfigured environment.

If you have an existing ARM Template or set of resources that you would like to convert to `.bicep` format, see [Decompiling an ARM Template](./docs/decompiling.md).

## How does Bicep work?

First, author your Bicep code using the Bicep language service as part of the [Bicep VS Code extension](./docs/installing.md#bicep-vs-code-extension)

Both [Az CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (2.20.0+) and the [PowerShell Az module](https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-5.5.0) (v5.6.0+) have Bicep support built-in. This means you can use the standard deployment commands with your `*.bicep` files and the tooling will transpile the code and send it to ARM on your behalf. For example, to deploy `main.bicep` to a resource group `my-rg`, we can use the CLI command we are already used to:

```bash
az deployment group create -f ./main.bicep -g my-rg
```

## How is life better with Bicep?

* Simpler syntax when compared to equivalent ARM Template JSON
  * No special `"[...]"` syntax required to write expressions
    * Directly call parameters or variables in expressions without a function (no more need for `parameters('myParam')`)
  * No quotes on property names (e.g. `"location"`)
  * String interpolation: `'${namePrefix}-vm'` instead of `concat(parameters('namePrefix'), '-vm')`
  * Direct `.` property access of a resource (e.g. `aks.properties.fqdn` instead of `reference(parameters('aksName')).properties.fqdn`)
* Easily break your Bicep project down into multiple files with [modules](https://github.com/Azure/bicep/blob/main/docs/spec/modules.md)
* Rich type validation and intellisense
  * Advanced type validation based on all Azure resource type API definitions
  * Completions on resource properties (`output sample string = resource.properties.*`)
* Automatic dependency management in certain scenarios. Bicep will automatically add `dependsOn` in the compiled ARM Template if the symbolic name is used in another resource declaration.

For more detail on taking advantage of new Bicep constructs that replace an equivalent from ARM Templates, you can read the [moving from ARM => Bicep](./docs/arm2bicep.md) doc.

## Known limitations

* No support for single-line object and arrays (i.e. `['a', 'b', 'c']`) ([#586](https://github.com/Azure/bicep/issues/586)).
* Bicep is newline sensitive. We are exploring ways we can remove/relax this restriction ([#146](https://github.com/Azure/bicep/issues/146))

## FAQ

**Why create a new language instead of using an existing one?**
Bicep is more of a revision to the existing ARM template language rather than an entirely new language. While most of the syntax has been changed, the core functionality of ARM templates and the runtime remains the same. You have the same template functions, same resource declarations, etc. Part of the complexity with ARM Templates is due to the "DSL" being embedded inside of JSON. With Bicep, we are revising the syntax of this DSL and moving it into its own `.bicep` file format. Before going down this path, we closely evaluated using an existing high-level programming language, but ultimately determined that Bicep would be easier to learn for our target audience. We are open to other implementations of Bicep in other languages.

**Why not focus your energy on Terraform or other third-party IaC offerings?**
Using terraform can be a great choice depending on the requirements of the organization, and if you are happy using terraform there is no reason to switch. At Microsoft, we are actively investing to make sure the terraform on Azure experience is the best it can be.

That being said, there is a huge customer base using ARM templates today because it provides a unique set of capabilities and benefits. We wanted to make the experience for those customers first-class as well, in addition to making it easier to start for Azure focused customers who have not yet transitioned to infra-as-code.

Fundamentally, we believe that configuration languages and tools are always going to be polyglot and different users will prefer different tools for different situations. We want to make sure all of these tools are great on Azure, Bicep is only a part of that effort.

**Is this ready for production use?**
Yes. As of v0.3, Bicep is now supported by Microsoft Support Plans and Bicep has 100% parity with what can be accomplished with ARM Templates. As of this writing, there are no breaking changes currently planned, but **it is still possible they will need to be made in the future**.

**Is this only for Azure?**
Bicep is a DSL focused on deploying end-to-end solutions in Azure. In practice, that usually means working with some non-Azure APIs (i.e. creating Kubernetes deployments or users in a database), so we expect to provide some extensibility points. That being said, currently only Azure resources exposed through the ARM API can be created with Bicep.

**What happens to my existing ARM Template investments?**
One of our goals is to make the transition from ARM Templates to Bicep as easy as possible. The Bicep CLI supports a `decompile` command to generate Bicep code from an ARM template. Please see [Decompiling an ARM Template](https://github.com/Azure/bicep/blob/main/docs/decompiling.md) for usage information.

Note that while we want to make it easy to transition to Bicep, we will continue to support and enhance the underlying ARM Template JSON language. As mentioned in [What is Bicep?](#what-is-bicep), ARM Template JSON remains the wire format that will be sent to Azure to carry out a deployment.

## Reference

* [Complete language spec](./docs/spec/bicep.md)
* [@BicepLang](https://twitter.com/BicepLang)
* [ARM Template Reference](https://docs.microsoft.com/azure/templates/)

## Community Bicep projects

* [Bicep GitHub Action](https://github.com/marketplace/actions/bicep-build)
* [Bicep Language Service support in Neovim](https://github.com/Azure/bicep/issues/1141#issuecomment-749372637)
* [Bicep PowerShell Module](https://github.com/StefanIvemo/BicepPowerShell)
* [Bicep Tasks extension for Azure Pipelines](https://marketplace.visualstudio.com/items?itemName=piraces.bicep-tasks)

## Alternatives

Because we are now treating the ARM Template as an IL, we expect and encourage other implementations of IL (ARM Template) generation. We'll keep a running list of alternatives for creating ARM templates that may better fit your use case.

* [Farmer](https://compositionalit.github.io/farmer/) (@isaacabraham) - Generate and deploy ARM Templates on .NET
* [Cloud Maker](https://cloudmaker.ai) (@cloud-maker-ai) - Draw deployable infrastructure diagrams that are converted to ARM templates

## Contributing
See [Contributing to Bicep](./CONTRIBUTING.md) for information on building/running the code, contributing code, contributing examples and contributing feature requests or bug reports.
