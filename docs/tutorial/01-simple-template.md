# Working with a basic bicep file

In this tutorial we'll start from a blank file and build up to a file with the basic bicep primitives.

Ensure you have met the following prerequisites:
* Installed Bicep CLI and VS Code extension ([installation instructions](../installing.md))
* Az CLI v2.20.0 or later installed
* An Azure subscription and resource group available to create deployments (this tutorial uses a resource group called `my-rg`)

## Add a resource

Let's start by creating a blank file `main.bicep`

Next we will add our first `resource` to our bicep file -- a basic storage account.

```
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
* **symbolic name** (`stg`) - this is an identifier for referencing the resource throughout your bicep file. It is *not* what the name of the resource will be when it's deployed.
* **type** (`Microsoft.Storage/storageAccounts@2019-06-01`) - composed of the resource provider (`Microsoft.Storage`), resource type (`storageAccounts`), and apiVersion (`2019-06-01`). These properties should be familiar if you've ever deployed ARM Templates before. You can find more types and apiVersions for various Azure resources [here](https://docs.microsoft.com/en-us/rest/api/resources/).
* **properties** (everything inside `= {...}`) - these are the specific properties you would like to specify for the given resource type. These are *exactly* the same properties available to you in an ARM Template.

## Add parameters

In most cases, I want to expose the resource name and the resource location via parameters, so I can add the following:

```
param location string = 'eastus'
param name string = 'uniquestorage001' // must be globally unique

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: name
  location: location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}
```

Notice the `parameters` can be referenced directly via their name in bicep, compared to requiring `[parameters('location')]` in ARM template JSON.

The end of the parameter declarations (`= 'eastus', = 'uniquestorage001'`) are *default* values. They can be optionally overridden at deployment time.

## Add variables and outputs

I can also add `variables` for storing values or complex expressions, and emit `outputs` to be passed to a script or another bicep file:

```
param location string = 'eastus'
param name string = 'uniquestorage001' // must be globally unique

var storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: name
  location: location
  kind: 'Storage'
  sku: {
    name: storageSku // reference variable
  }
}

output storageId string = stg.id // output resourceId of storage account
```

Notice I can easily reference the resource `id` from the symbolic name of the storage account (`stg.id`). No need to use the `resourceId()` function.

## Next steps

In the next tutorial, we will walk through compiling and deploying a bicep file:

[2 - Deploying a bicep file](./02-deploying-a-bicep-file.md)
