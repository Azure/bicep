[![Build](https://github.com/Azure/bicep/actions/workflows/build.yml/badge.svg)](https://github.com/Azure/bicep/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/Azure/bicep/branch/main/graph/badge.svg)](https://codecov.io/gh/Azure/bicep)
[![Good First Issues](https://img.shields.io/github/issues/Azure/Bicep/good%20first%20issue?color=important&label=good%20first%20issue&style=flat)](https://github.com/Azure/Bicep/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)
[![Needs Feedback](https://img.shields.io/github/issues/Azure/Bicep/needs%20feedback?color=blue&label=needs%20feedback%20&style=flat)](https://github.com/Azure/bicep/issues?q=is%3Aopen+is%3Aissue+label%3A%22needs+feedback%22)

# Project Bicep - an ARM DSL

## What is Bicep?

Bicep is a Domain Specific Language (DSL) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax, improved type safety, and better support for modularity and code re-use. Bicep is a **transparent abstraction** over ARM and ARM templates, which means anything that can be done in an ARM Template can be done in Bicep (outside of temporary [known limitations](#known-limitations)). All resource `types`, `apiVersions`, and `properties` that are valid in an ARM template are equally valid in Bicep on day one (Note: even if Bicep warns that type information is not available for a resource, it can still be deployed).




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
2. **Complete the [Bicep Learning Path](https://docs.microsoft.com/learn/paths/bicep-deploy/)**

Alternatively, you can try the [Bicep Playground](https://aka.ms/bicepdemo) or use the [VS Code Devcontainer/Codespaces](https://github.com/Azure/vscode-remote-try-bicep) repo to get a preconfigured environment.

If you have an existing ARM Template or set of resources that you would like to convert to `.bicep` format, see [Decompiling an ARM Template](https://docs.microsoft.com/azure/azure-resource-manager/bicep/decompile).

Full details of how the bicep language works can be found in the [Bicep documentation](https://docs.microsoft.com/azure/azure-resource-manager/bicep/) and there is a rich library of [examples](https://github.com/Azure/bicep/tree/main/docs/examples) to help you get a jumpstart.

## How does Bicep work?

First, author your Bicep code using the Bicep language service as part of the [Bicep VS Code extension](./docs/installing.md#bicep-vs-code-extension)

Both [Az CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (2.20.0+) and the [PowerShell Az module](https://docs.microsoft.com/en-us/powershell/azure/install-az-ps) (v5.6.0+) have Bicep support built-in. This means you can use the standard deployment commands with your `*.bicep` files and the tooling will transpile the code and send it to ARM on your behalf. For example, to deploy `main.bicep` to a resource group `my-rg`, we can use the CLI command we are already used to:

```bash
az deployment group create -f ./main.bicep -g my-rg
```

For more detail on taking advantage of new Bicep constructs that replace an equivalent from ARM Templates, you can read the [moving from ARM => Bicep](./docs/arm2bicep.md) doc.

## Known limitations

* No support for single-line object and arrays (i.e. `['a', 'b', 'c']`) ([#586](https://github.com/Azure/bicep/issues/586)).
* Bicep is newline sensitive. We are exploring ways we can remove/relax this restriction ([#146](https://github.com/Azure/bicep/issues/146))
* No support for the concept of apiProfile which is used to map a single apiProfile to a set apiVersion for each resource type. We are looking to bring support for this type of capability, but suspect it will work slightly differently. Discussion is in [#622](https://github.com/Azure/bicep/issues/622)

## FAQ

**What unique benefits do you get with Bicep?**

1. Day 0 resource provider support. Any Azure resource — whether in private or public preview or GA — can be provisioned using Bicep.
2. Much simpler syntax [compared to equivalent ARM Template JSON](./docs/arm2bicep.md)
3. No state or state files to manage. All state is stored in Azure, so makes it easy to collaborate and make changes to resources confidently. 
4. Tooling is the cornerstone to any great experience with a programming language. Our VS Code extension for Bicep makes it extremely easy to author and get started with advanced type validation based on all Azure resource type [API definitions](https://github.com/Azure/azure-rest-api-specs/tree/master/specification).
5. Easily break apart your code with native [modules](./docs/spec/modules.md) 
6. Supported by Microsoft support and 100% free to use.

**Why create a new language instead of using an existing one?**

Bicep is more of a revision to the existing ARM template language rather than an entirely new language. While most of the syntax has been changed, the core functionality of ARM templates and the runtime remains the same. You have the same template functions, same resource declarations, etc. Part of the complexity with ARM Templates is due to the "DSL" being embedded inside of JSON. With Bicep, we are revising the syntax of this DSL and moving it into its own `.bicep` file format. Before going down this path, we closely evaluated using an existing high-level programming language, but ultimately determined that Bicep would be easier to learn for our target audience. We are open to other implementations of Bicep in other languages.

We spent a lot of time researching various different options and even prototyped a TypeScript based approach. We did over 120 customer calls, Microsoft Most Valuable Professional (MVP) conversations and collected quantitative data. We learned that in majority of organizations, it was the cloud enablement teams that were responsible for provisioning the Azure infra. These folks were not familiar with programming languages and did not like that approach as it had a steep learning curve. These users were our target users. In addition, authoring ARM template code in a higher level programming language would require you to reconcile two uneven runtimes, which ends up being confusing to manage. At the end of the day, we simply want customers to be successful on Azure. In the future if we hear more feedback asking us to support a programming language approach, we are open to that as well. If you'd like to use a high-level programming language to deploy Azure Infra we recommend [Farmer](https://compositionalit.github.io/farmer/) or [Pulumi](https://www.pulumi.com/).

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

## Get Help, Report an issue
We are here to help you be successful with Bicep, please do not hesitate to reach out to us. 

* If you need help or have a generic question such as ‘where can I find an example for…’ or ‘I need help converting my ARM Template to Bicep’ you can [open a discussion]( https://github.com/Azure/bicep/discussions)
* If you have a bug to report or a new feature request for Bicep please [open an issue]( https://github.com/Azure/bicep/issues)

## Reference

* [Complete language spec](./docs/spec/bicep.md)
* [@BicepLang](https://twitter.com/BicepLang)
* [ARM Template Reference](https://docs.microsoft.com/azure/templates/)

## Community Bicep projects

* [Bicep GitHub Action](https://github.com/marketplace/actions/bicep-build)
* [Bicep Language Service support in Neovim](https://github.com/Azure/bicep/issues/1141#issuecomment-749372637)
* [Bicep PowerShell Module](https://github.com/PSBicep/PSBicep)
* [Bicep Tasks extension for Azure Pipelines](https://marketplace.visualstudio.com/items?itemName=piraces.bicep-tasks)

## Alternatives

Because we are now treating the ARM Template as an IL, we expect and encourage other implementations of IL (ARM Template) generation. We'll keep a running list of alternatives for creating ARM templates that may better fit your use case.

* [Farmer](https://compositionalit.github.io/farmer/) (@isaacabraham) - Generate and deploy ARM Templates on .NET
* [Cloud Maker](https://cloudmaker.ai) (@cloud-maker-ai) - Draw deployable infrastructure diagrams that are converted to ARM templates

## Telemetry

When using the Bicep VS Code extension, VS Code collects usage data and sends it to Microsoft to help improve our products and services. Read our [privacy statement](https://go.microsoft.com/fwlink/?LinkID=528096&clcid=0x409) to learn more. If you don’t wish to send usage data to Microsoft, you can set the `telemetry.enableTelemetry` setting to `false`. Learn more in our [FAQ](https://code.visualstudio.com/docs/supporting/faq#_how-to-disable-telemetry-reporting).

## License
All files except for the [Azure Architecture SVG Icons](./src/vscode-bicep/src/visualizer/app/assets/icons/azure) in the repository are subject to the [MIT license](./LICENSE).

The [Azure Architecture SVG Icons](./src/vscode-bicep/src/visualizer/app/assets/icons/azure) used in the Bicep VS Code extension are subject to the [Terms of Use](https://docs.microsoft.com/en-us/azure/architecture/icons/#terms).

## Contributing
See [Contributing to Bicep](./CONTRIBUTING.md) for information on building/running the code, contributing code, contributing examples and contributing feature requests or bug reports.
