# Creating and consuming modules

Now that we have our immensely useful bicep file for any number of storage blob containers, we may want to share it or use it as part of a larger bicep project. To do that, we can use `modules`.

Let's start by renaming our `main.bicep` file to `storage.bicep`. This is the file we will use as a `module`. Any bicep file can be a module, so there are no other syntax changes we need to make to this file. It can already be used as a module.

Next let's create a new, empty `main.bicep` file. Your file structure should look like this:

```bash
.
├── main.bicep
└── storage.bicep
```

In `main.bicep`, we'll add the following code to instantiate our module:

```bicep
module stg './storage.bicep' = {
  name: 'storageDeploy'
  params: {
    storageAccountName: '<YOURUNIQUESTORAGENAME>'
  }
}
```

`storageAccountName` is the only required parameter since it does not have a default value. The other `params` are not required, but they can still be optionally overridden. 

>**Note:** All modules require a `name` property that is the name of the nested deployment (`microsoft.resources/deployments`) in the compiled ARM Template.

Notice modules, just like everything in bicep, has an identifier and we can use that identifier to retrieve information like `outputs` from the module. We can add an output to `main.bicep` to retrieve the storage account name:

```bicep
module stg './storage.bicep' = {
  name: 'storageDeploy'
  params: {
    storageAccountName: '<YOURUNIQUESTORAGENAME>'
  }
}

output storageName string = stg.outputs.computedStorageName
```

## Deploying modules to a different scope

Modules also support a `scope` property, which allows you to specify a scope that is different than the target scope of the deployment. For example, we may want the storage module to be deployed to a different resource group. To do so, let's add the `scope` property to our module declaration:

```bicep
module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('another-rg') // this will target another resource group in the same subscription
  params: {
    storageAccountName: '<YOURUNIQUESTORAGENAME>'
  }
}

output storageName array = stg.outputs.containerProps
```

Bicep files assume that the target scope of the deployment is a resource group, but you can override this as well. Let's make a final change to our `main.bicep` file to change the `targetScope` to `subscription` and create a simple role assignment resource.

```bicep
targetScope = 'subscription'

module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('another-rg') // this will target another resource group in the same subscription
  params: {
    storageAccountName: '<YOURUNIQUESTORAGENAME>'
  }
}

var objectId = 'cf024e4c-f790-45eb-a992-5218c39bde1a' // change this AAD object ID. This is specific to the microsoft tenant
var contributor = 'b24988ac-6180-42a0-ab88-20f7382dd24c'
resource rbac 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(subscription().id, objectId, contributor)
  properties: {
    roleDefinitionId: contributor
    principalId: objectId
  }
}

output storageName array = stg.outputs.containerProps
```

Since the `targetScope` has been changed to `subscription`, I will need to use the subscription deployment command (`az deployment sub create ...`).

Modules are a powerful way to separate your bicep files into logical units and abstract away complex resource declarations.

## Using if() and for keywords with bicep Modules

Modules also support both the `if` and `for` keywords, just like a `resource` does. Let's add a new `param` called `deployStorage` and use it conditionally deploy the storage account module:

```bicep
param deployStorage bool = true

module stg './storage.bicep' = if (deployStorage) {
  name: 'storageDeploy'
}
```

In this example if the parameter of `deployStorage` is set to `false` then everything in the module will be skipped at deployment. If `deployStorage` is set to `true` then the module will be deployed. Combining modules with the usage of the `if()` condition allows for customizing deployed components based on specific criteria/parameters.

If instead I want to iterate and create multiple version of the module, I can use the same `for` syntax we just learned:

```bicep
param deployments array = [
  'foo'
  'bar'
]

module stg './storage.bicep' = [for item in deployments: {
  name: '${item}storageDeploy'
}]
```

## Next steps

In the final tutorial, we will learn how to convert an arbitrary ARM template into a bicep file:

[7 - Convert any ARM template into a Bicep file](./07-convert-arm-template.md)
