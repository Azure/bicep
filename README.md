[![Build](https://github.com/Azure/bicep/actions/workflows/build.yml/badge.svg)](https://github.com/Azure/bicep/actions/workflows/build.yml)
[![Test Azure CLI Integration](https://github.com/Azure/bicep/actions/workflows/test-azure-cli-integration.yml/badge.svg?branch=main)](https://github.com/Azure/bicep/actions/workflows/test-azure-cli-integration.yml)
[![Needs Upvote](https://img.shields.io/github/issues/Azure/Bicep/Needs%3A%20Upvote?color=green&label=Needs%3A%20Upvote&style=flat)](https://github.com/Azure/bicep/issues?q=is%3Aopen+is%3Aissue+label%3A%22Needs%3A+Upvote%22+sort%3Areactions-%2B1-asc)
[![Good First Issues](https://img.shields.io/github/issues/Azure/Bicep/good%20first%20issue?color=blue&label=good%20first%20issue&style=flat)](https://github.com/Azure/Bicep/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)

# Azure Bicep

For all you need to know about the Bicep language, check out the [Bicep documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/).

## What is Bicep?

Bicep is an infrastructure-as-code (IaC) programming language that uses declarative syntax to deploy Azure resources. In a Bicep file, you define the infrastructure you want to deploy to Azure and then use that file throughout the development lifecycle to repeatedly deploy that infrastructure. Your resources are deployed in a consistent manner.

Learn more about the language and benefits of using Bicep in the [What is Bicep documentation](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep).

## Get started with Bicep

To get started with Bicep:

1. **[Install the tooling](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install).**
1. **Complete the [Fundamentals of Bicep Learning Path](https://learn.microsoft.com/en-us/training/paths/fundamentals-bicep/).**
1. **See the full list of [Learn modules for Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/learn-bicep).**

> [!TIP]
> If you're new to Bicep, start by installing the Bicep VS Code extension and deploying a simple resource (like a storage account) to get comfortable with the end-to-end workflow before moving to larger templates.

If you have an existing Azure Resource Manager (ARM) template or set of Azure resources that you want to convert to `.bicep` format, see the [recommended workflow for migrating resources to Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/migrate) and [Decompiling an ARM Template](https://learn.microsoft.com/azure/azure-resource-manager/bicep/decompile).

Also, there's a rich library of [Bicep modules in Azure Verified Modules](https://azure.github.io/Azure-Verified-Modules/indexes/bicep/) and examples in the [azure-quickstart-templates](https://github.com/Azure/azure-quickstart-templates) repo to help you get started. You can also use the [Bicep Playground](https://azure.github.io/bicep/) to try out Bicep in your browser.

If you're looking for production-ready and tested Bicep templates, you can find them in the [bicep-registry-modules](https://github.com/Azure/bicep-registry-modules) repo. Learn more about these templates on the Azure Verified Modules website: [https://aka.ms/avm](https://aka.ms/avm).

## How does Bicep work?

First, author your Bicep code by using the Bicep language service as part of the [Bicep VS Code extension](https://learn.microsoft.com/azure/azure-resource-manager/bicep/install#vs-code-and-bicep-extension).

Both [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) (2.20.0+) and the [PowerShell Az module](https://learn.microsoft.com/en-us/powershell/azure/install-azure-powershell) (v5.6.0+) have Bicep support built in. This support means you can use the standard deployment commands with your `*.bicep` files. The tooling transpiles the code and sends it to ARM on your behalf. For example, to deploy `main.bicep` to a resource group `my-rg`, use the CLI command:

```bash
az deployment group create -f ./main.bicep -g my-rg
```

## Goals

1. Build the best possible language for describing, validating, and deploying infrastructure to Azure.
1. The language should provide a *transparent abstraction* for the underlying platform. There must be no "onboarding step" to enable Bicep support for a new resource type or API version.
1. Code should be easy to understand at a glance and straightforward to learn, regardless of your experience with other programming languages.
1. Users should have a lot of freedom to modularize and re-use their code. Code re-use shouldn't require any 'copy/paste'-ing.
1. Tooling should provide a high level of resource discoverability and validation. Develop it alongside the compiler rather than adding it at the end.
1. Users should have a high level of confidence that their code is 'syntactically valid' before deploying.

### Non-goals

1. Build a general-purpose language to meet any need. This language doesn't replace general-purpose languages. You might still need to do pre or post-Bicep execution tasks in a script or high-level programming language.
1. Provide a first-class provider model for non-Azure related tasks. While the [extensibility model](https://github.com/Azure/bicep-extensibility) supports [Microsoft Graph](https://learn.microsoft.com/en-us/graph/templates/bicep/overview-bicep-templates-for-graph), official extension points are intended to be focused on Azure infra or application deployment related tasks.

## FAQ

**What unique benefits do you get with Bicep?**

1. Day 0 resource provider support. You can use Bicep to provision any ARM resource, whether it's in private, public preview, or GA.
1. Much simpler syntax [compared to equivalent ARM Template JSON](https://learn.microsoft.com/azure/azure-resource-manager/bicep/compare-template-syntax).
1. No state or state files to manage. All state is stored in Azure, so it's easy to collaborate and make changes to resources confidently.
1. Tooling is the cornerstone to any great experience with a programming language. The VS Code extension for Bicep makes it extremely easy to author and get started with advanced type validation based on all Azure resource type [API definitions](https://github.com/Azure/azure-rest-api-specs/tree/main/specification).
1. Easily break apart your code with native [modules](https://learn.microsoft.com/azure/azure-resource-manager/bicep/modules).
1. Supported by Microsoft support and 100% free to use.

**[See more frequently asked questions on Microsoft Learn](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/frequently-asked-questions)**

## Get Help, Report an issue

The Bicep team is here to help you be successful with Bicep. Don't hesitate to reach out.

* For help or generic questions such as "where can I find an example for..." or "I need help converting my ARM Template to Bicep," [open a discussion](https://github.com/Azure/bicep/discussions).
* For bugs or new feature requests for Bicep, [open an issue](https://github.com/Azure/bicep/issues).

You can see the state of the issues in [our GitHub Project](https://github.com/orgs/Azure/projects/115).

### Community call

The Bicep team hosts an open community call for users of Bicep. Go [here](https://aka.ms/armnews) to get invited, and follow [the labeled issues](https://github.com/Azure/bicep/issues?q=sort%3Aupdated-desc%20is%3Aissue%20state%3Aopen%20label%3A%22Community%20Call%22) for updates.

You can find recordings of the community calls on the [Azure Deployments & Governance YouTube Channel](https://www.youtube.com/channel/UCZZ3-oMrVI5ssheMzaWC4uQ/videos).

### Social media

You can also find us on social media on the following platforms:

* [@BicepLang (X)](https://twitter.com/BicepLang)
* [@BicepLang (Bluesky)](https://bsky.app/profile/biceplang.bsky.social)

You can also get in touch with other users through the community-created [Azure Bicep users group on LinkedIn](https://www.linkedin.com/groups/13004126/).

## Reference

* [Complete language spec](https://learn.microsoft.com/azure/azure-resource-manager/bicep/file)
* [Azure Resources Reference](https://learn.microsoft.com/azure/templates/)
* [Bicep CLI commands](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-cli)
* [Azure Bicep Deploy GitHub Action](https://github.com/Azure/bicep-deploy)

## Community Bicep projects

* [Awesome Bicep list](https://github.com/ElYusubov/AWESOME-Azure-Bicep)
* [Bicep Language Service support in Neovim](https://github.com/Azure/bicep/issues/1141#issuecomment-749372637)
* [Bicep PowerShell Module](https://github.com/PSBicep/PSBicep)
* [Bicep Tasks extension for Azure Pipelines](https://marketplace.visualstudio.com/items?itemName=piraces.bicep-tasks)

## Alternatives

Because the ARM Template is now treated as an IL, other implementations of IL (ARM Template) generation are expected and encouraged. This list provides alternatives for creating ARM templates that might better fit your use case.

* [Farmer](https://compositionalit.github.io/farmer/) (@isaacabraham) - Generate and deploy ARM Templates on .NET
* [Cloud Maker](https://cloudmaker.ai) (@cloud-maker-ai) - Draw deployable infrastructure diagrams that are converted to ARM templates or Bicep

## Telemetry

When you use the Bicep VS Code extension, VS Code collects usage data and sends it to Microsoft to help improve our products and services. Read the [privacy statement](https://go.microsoft.com/fwlink/?LinkID=528096&clcid=0x409) to learn more. If you don't want to send usage data to Microsoft, set the `telemetry.enableTelemetry` setting to `false`. For more information, see the [FAQ](https://code.visualstudio.com/docs/supporting/faq#_how-to-disable-telemetry-reporting).

## License

All files in the repository, except for the Azure Architecture SVG Icons ([here](./src/vscode-bicep/src/visualizer/app/assets/icons/azure) and [here](./src/vscode-bicep-ui/packages/components/assets/azure-architecture-icons)), are subject to the [MIT license](./LICENSE).

The Azure Architecture SVG Icons ([here](./src/vscode-bicep/src/visualizer/app/assets/icons/azure) and [here](./src/vscode-bicep-ui/packages/components/assets/azure-architecture-icons)) used in the Bicep VS Code extension are subject to the [Terms of Use](https://learn.microsoft.com/azure/architecture/icons/#terms).

## Contributing

For information on building and running the code, contributing code, contributing examples, and contributing feature requests or bug reports, see [Contributing to Bicep](./CONTRIBUTING.md).
