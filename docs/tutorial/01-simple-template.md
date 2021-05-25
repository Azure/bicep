# Working with a basic Bicep file

In this tutorial we'll start from a blank file and build up to a file with the basic Bicep primitives.

Ensure you have met the following prerequisites:

* Bicep CLI and VS Code extension installed ([installation instructions](../installing.md))
* Az CLI v2.20.0 or later installed ([installation instructions](https://docs.microsoft.com/cli/azure/install-azure-cli))
* An Azure subscription and resource group available to create deployments (this tutorial uses a resource group called `my-rg`)

## Add a resource

Let's start by creating a blank file `main.bicep` and add our first `resource` to it -- a basic storage account.

```bicep
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'uniquestorage001' // must be globally unique
  location: 'eastus'
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}
```

The resource declaration has four components:

* `resource` keyword
* **symbolic name** (`stg`) - this is an identifier for referencing the resource throughout your Bicep file. It is *not* the name of the resource that will be deployed.
* **type** (`Microsoft.Storage/storageAccounts@2019-06-01`) - composed of the resource provider (`Microsoft.Storage`), resource type (`storageAccounts`), and api version (`2019-06-01`). These properties should be familiar if you have deployed ARM Templates before. You can browse [full list of types](https://docs.microsoft.com/rest/api/resources/) for more Azure resource types and versions.
* **properties** (everything inside `= {...}`) - these are the specific properties you would like to specify for the given resource type. These are *exactly* the same properties available to you in an ARM Template.

## Add parameters

In most cases, I want to expose the resource name and the resource location via parameters, so I can add the following:

```bicep
param location string = 'eastus'
param storageAccountName string = 'uniquestorage001' // must be globally unique

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}
```

The end of the parameter declarations (`= 'eastus'`, `= 'uniquestorage001'`) are *default* values. They can be optionally overridden at deployment time.

Notice the `parameters` can be referenced directly via their name in Bicep, compared to requiring `[parameters('location')]` in ARM template JSON.

I can use any number of parameter decorators to augment more behavior of the parameter, such as restricting the length. Decorators use the `@` character and must be declared directly above the symbol it is decorating.  

Storage account names must be between 3 and 24 characters, so let's add those restrictions to our `name` parameter.

```bicep
@minLength(3)
@maxLength(24)
param storageAccountName string = 'uniquestorage001' // must be globally unique
```

## Add variables and outputs

I can also add `variables` for storing values or complex expressions, and emit `outputs` to be passed to a script or another Bicep file:

```bicep
param location string = 'eastus'

@minLength(3)
@maxLength(24)
param storageAccountName string = 'uniquestorage001' // must be globally unique

var storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'Storage'
  sku: {
    name: storageSku // reference variable
  }
}

output storageId string = stg.id // output resourceId of storage account
```

Notice I can easily reference the resource `id` from the symbolic name of the storage account (`stg.id`). No need to use the `resourceId()` function. This is one of the many uses of the resource identifier that will be covered more deeply in Tutorial 03.

## Next steps

In the next tutorial, we will walk through deploying a Bicep file:

[2 - Deploying a bicep file](./02-deploying-a-bicep-file.md)
