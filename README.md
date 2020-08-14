# Project Bicep - an ARM DSL

*FYI, all of the details of this readme are subject to change*

## What is Bicep?

Bicep is a Domain Specific Lanuage (DSL) for deploying Azure resources declaratively. Bicep compiles down to "ARM" JSON in a 1:1 manner, such that you can "decompile" back to bicep, which means the ARM JSON is being treated as an Intermediate Language (IL).

Bicep is a **transparent abstraction** over ARM and ARM templates, so all of your knowledge of how resources are declared in a template will carry over to Bicep. All `resourceType`, `apiVersions`, and resource `properties` that are valid in an ARM template are equally valid in Bicep.

## High Level Design Goals
1. Build the best possible language for describing, validating, and deploying infrastructure to Azure
1. The language should provide a 'transparent abstraction' for the underlying platform. There must be no "onboarding step" to enable it to a new `resourceType` or `apiVersion` in Bicep.
1. Code should be easy to understand at a glance and straightforward to learn, regardless of your experience with other programming languages.
1. Users should be given a lot of freedom to modularize and reuse their code if they desire. Reusing code should not require any 'copy/paste'.
1. Tooling should provide a high level of resource discoverability and validation, and should be developed alongside the compiler rather than added at the end.
1. Users should have a high level of confidence that their code is 'valid' before deploying.

## Non-goals
1. Build a general purpose language to meet any need. This will not replace general purpose languages and you may still need to do pre or post bicep tasks for niche work. 
1. Provide a first-class provider model for non-Azure related tasks. While we will likely introduce an extensability model at some point, any extension points are intended to be focused on Azure infra or application related tasks.

## How does Bicep work?

First, author your Bicep code using the Bicep language service as part of the [Bicep VS Code extension](https://github.com/Azure/bicep/actions), then compile that code into an ARM template. This can happen in a few different ways:

**compile yourself** 
  
`bicep build` or `bicep watch`
  
By convention, these commands will look for `main.arm` and will generate a file called `azureDeploy.json` in the same directory.

At this point, you can use the generated template as you would normally - test with ARM TTK, validate with what-if, deploy via pipelines, etc.

**compile and deploy**

`bicep deploy` 

This will run `bicep build` and then deploy the generated template via standard CLI tooling. Design of this is still [ongoing](https://github.com/Azure/bicep/issues/33).

## How is life better with Bicep?

* Much simpler syntax when compared to equivalent JSON
  - no special `[...]` expressions syntax required
      * directly call parameters or variables in expressions without a function (no more need for `parameters('myParam')`)
  - no quotes on property names (e.g. `"location"`)
  - simple string interpolation: `'${namePrefix}-vm'` instead of `concat(parameters('namePrefix'), '-vm')`
  - simpler resource declaration using positional properties to avoid typing common property names like `resourceType` and `apiVersion` explicitly.
  - Direct property access of a resource (e.g. `aks.properties.fqdn` instead of `reference(parameters('aksName')).properties.fqdn`)
* [**Modules**](#why-are-modules-so-important), which allow you to simplify declarations of a single resource or set of resources in separate files.
* Better copy/paste experience via flexible declaration of types. Different types (e.g. `variables`, `resources`, `outputs`) can be declared anywhere.
  - previously all parameters had to be declared together in one `"parameters": {}` object, variables had to be declared together in one `"variables": {}` object, etc.
* Simpler declaration of [conditional statements](./docs/spec/resources.md#conditions) and [loops](./docs/spec/loops.md)
* Semi-automatic dependency management. If a resource identifier is used in another resource declaration, the dependency will be added automatically.


## Syntax basics

To declare a resource you use the `resource` keyword:

```
resource <identifier> <type@apiVersion> = {
  // properties
}
```

Here's a simple virtual network:

```
resource storage 'Microsoft.Storage/storageAccounts@2020-01-01` = {
  name: 'uniquestorage001'
  location: 'eastus'
  sku: {
      name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
```

Notice the `resourceType` and `apiVersion` are declared in a single string, separated by `@`.

Just like ARM templates, the resource declaration still requires **exactly** what the `PUT` API is expecting to create or update the resource. If you wish to abstract away certain required properties, you can use [modules](#why-are-modules-so-important).

I can use both parameters and variables to make my bicep template more dynamic:

```
parameter storageName string // can set default value by adding "= 'mydefault'" 

variable location = 'eastus'

resource storage 'Microsoft.Storage/storageAccounts@2020-01-01` = {
  name: storageName
  location: location
  sku: {
      name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
```

For more details and examples, check out the below spec for each high level concept in Bicep.

Full bicep spec:
* [Resources](https://github.com/Azure/bicep/blob/master/docs/spec/resources.md)
* [Parameters](https://github.com/Azure/bicep/blob/master/docs/spec/parameters.md)
* [Variables](https://github.com/Azure/bicep/blob/master/docs/spec/variables.md)
* [Outputs](https://github.com/Azure/bicep/blob/master/docs/spec/outputs.md)
* [Expressions**](https://github.com/Azure/bicep/blob/master/docs/spec/expressions.md)
* [Loops](https://github.com/Azure/bicep/blob/master/docs/spec/loops.md)
* [Conditions](https://github.com/Azure/bicep/blob/master/docs/spec/resources.md#conditions)
* [Modules](https://github.com/Azure/bicep/blob/master/docs/spec/modules.md)

**not closed on design

## Why are modules so important?

For better or worse, the ARM APIs are complicated. There are a lot of different `resourceTypes` and each `resourceType` has it's own [specification](https://github.com/Azure/azure-rest-api-specs/tree/master/specification) for what properties it exposes. Those resourceType APIs evolve over time, which introduces the notion of `apiVersions`. All of this makes Azure resources, and codifying those resources into an ARM Template (or equivalent), much more complicated.

To help alleviate this pain, we are introducing the concept of `modules`, which is a first class bicep construct to help you simplify the declaration of resources.

For example, I have an AKS cluster that I want to deploy, which at the lowest level, requires a declaration like this:

```
parameter clusterName string
...

resource aks 'Microsoft.ContainerService/managedClusters@2020-03-01' = {
    name: clusterName
    location: location
    properties: {
        dnsPrefix: dnsPrefix
        agentPoolProfiles: [
            {
                name: 'agentpool'
                osDiskSizeGB: osDiskSizeGB
                vmSize: agentVMSize
                osType: 'Linux'
                storageProfile: 'ManagedDisks'
            }
        ]
        linuxProfile: {
            adminUsername: linuxAdminUsername
            ssh: {
                publicKeys: [
                    {
                        keyData: sshRSAPublicKey
                    }
                ]
            }
        }
        servicePrincipalProfile: {
            clientId: servcePrincipalClientId
            secret: servicePrincipalClientSecret
        }
    }
}
```

This has lots of properties that I may want to override, but in many cases a `defaultValue` will do just fine. With a module, we can allow for a resource to be declared with only the absolute must-have properties like this:

```
module aksMod './modules/aks@1.0.0' = { // modules, optionally, can be versioned
  name: 'myAksCluster'
  location: resourceGroup().location
  clientId: 'mySpnId'
  secret: 'mySpnP@ssw0rd'
}
``` 

All of the heavy lifting is done by the module author, whose job is to design a simple interface for inputting the necessary info for the module to deploy successfully. You can [view the module definition here](http://path.to/module-definition.arm).

A module can deploy a single resource, or a **set of resources** representing a logical "solution". For example, I may have a backend service that requires an API App and Cosmos Database. I can expose the entire solution as [a single module](http://path.to/module-defintion.arm).

By default, the module path will target the [**Bicep Module Library**](http://path-to/bicep-module-library), which contains Microsoft Azure approved modules for a wide variety of commonly used resources. If you don't see a module for a resource you are interested in, you can open an issue or submit a PR. Modules can also reference any public url pointing to a directory with a `main.arm` file or by referencing the main.arm file directly.

```
module myCustomMod 'github.com/alex-frankel/web-app-container@1.0.0' = {
  name: 'myWebApp'
  location: resourceGroup().location
  containerName: my-container-app
}
```

## How do I go from ARM JSON to Bicep?

We will provide a decompiler to translate ARM JSON as part of our [0.3 release](https://github.com/Azure/bicep/issues/9). In the meantime, this transition must be done manually.


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
