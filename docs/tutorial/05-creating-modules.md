# Creating and consuming modules

Now that we have our immensely useful bicep file for creating a storage account and container, we may want to share it or use it as part of a larger bicep project. To do that, we can use `modules`.

Let's start by renaming our `main.bicep` file to `storage.bicep`. This is the file we will use as a `module`. Any bicep file can be a module, so there are no other syntax changes we need to make to this file. It can already be used as a module.

Next let's create a new, empty `main.bicep` file. Your file structure should look like this:

```bash
.
├── main.bicep
└── storage.bicep
```

In `main.bicep`, we'll add the following code to instantiate our module:

```
module stg './storage.bicep' = {
  name: 'storageDeploy'
  params: {
    namePrefix: 'contoso'
  }
}
```

Since our `storage.bicep` file has default values for all of the `params`, no `params` are required to be declared in the module. They can still be optionally overridden, which is what we've done with `namePrefix`. Let's compile `main.bicep` and look at the output.

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "functions": [],
  "resources": [
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2019-10-01",
      "name": "storageDeploy",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "namePrefix": {
            "value": "contoso"
          }
        },
        "template": {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "contentVersion": "1.0.0.0",
          "parameters": {
            "location": {
              "type": "string",
              "defaultValue": "[resourceGroup().location]"
            },
            "namePrefix": {
              "type": "string",
              "defaultValue": "stg"
            },
            "globalRedundancy": {
              "type": "bool",
              "defaultValue": true
            }
          },
          "functions": [],
          "variables": {
            "storageAccountName": "[format('{0}{1}', parameters('namePrefix'), uniqueString(resourceGroup().id))]"
          },
          "resources": [
            {
              "type": "Microsoft.Storage/storageAccounts",
              "apiVersion": "2019-06-01",
              "name": "[variables('storageAccountName')]",
              "location": "[parameters('location')]",
              "kind": "Storage",
              "sku": {
                "name": "[if(parameters('globalRedundancy'), 'Standard_GRS', 'Standard_LRS')]"
              }
            },
            {
              "type": "Microsoft.Storage/storageAccounts/blobServices/containers",
              "apiVersion": "2019-06-01",
              "name": "[format('{0}/default/logs', variables('storageAccountName'))]",
              "dependsOn": [
                "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
              ]
            }
          ],
          "outputs": {
            "storageId": {
              "type": "string",
              "value": "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
            },
            "computedStorageName": {
              "type": "string",
              "value": "[variables('storageAccountName')]"
            },
            "primaryEndpoint": {
              "type": "string",
              "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))).primaryEndpoints.blob]"
            }
          }
        }
      }
    }
  ]
}
```

When modules are transpiled into ARM template JSON, they are turned into a nested inline deployment. If you don't know anything about [nested deployments](https://docs.microsoft.com/azure/azure-resource-manager/templates/linked-templates), you don't need to understand them to leverage modules. At the end of the day, the same resources will be deployed. The `name` property of the `module` is the name we use for the `microsoft.resources/deployments` resource (the nested deployment) in the ARM template.

Notice modules, just like everything in bicep, has an identifier and we can use that identifier to retrieve information like `outputs` from the module. We can add an output to `main.bicep` to retrieve the storage account name:

```
module stg './storage.bicep' = {
  name: 'storageDeploy'
  params: {
    namePrefix: 'contoso'
  }
}

output storageName string = stg.outputs.computedStorageName
```

## Deploying modules to a different scope

Modules also support a `scope` property, which allows you to specify a scope that is different that the target scope of the deployment. For example, we may want the storage module to be deployed to a different resource group. To do so, let's add the `scope` property to our module declaration:

```
module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('another-rg') // this will target another resource group in the same subscription
  params: {
    namePrefix: 'contoso'
  }
}

output storageName string = stg.outputs.computedStorageName
```

Bicep files assume that the target scope of the deployment is a resource group, but you can override this as well. Let's make a final change our `main.bicep` file to change the `targetScope` to `subscription` and create a simple role assignment resource.

```
targetScope = 'subscription'

module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('another-rg') // this will target another resource group in the same subscription
  params: {
    namePrefix: 'contoso'
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

output storageName string = stg.outputs.computedStorageName
```

Since the `targetScope` has been changed, notice that the `$schema` property in the transpiled ARM Template has changed to the `...subscriptionDeploymentTemplate.json`.

Modules are a powerful way to separate your bicep files into logical units and abstract away complex resource declarations.

## Using if condition with bicep Modules
Modules also support the if condition (introduced in v0.2.212). The linked ARM template produced by `bicep build` will insert a condition at the deployment level for everything in the module. To take advantage of this the module declaration needs to follow the if condition syntax:
````
module stg './storage.bicep' = if (deployStorage) {
  name: 'storageDeploy'
}
````
In this example if the parameter of `deployStorage` is set to `false` then everything in the module will be skipped at deployment. If `deployStorage` is set to `true` the module will be deployed. The ARM template created by running `bicep build` will look like:
````
 {
      "condition": "[parameters('deployStorage')]",
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2019-10-01",
      .....
````

Thus combining modules with the usage of the if() condition allows for customizing deployed components based on criteria and/or parameters.


## Next steps

In the final tutorial, we will learn how to convert an arbitrary ARM template into a bicep file:

[6 - Convert any ARM template into a Bicep file](./06-convert-arm-template.md)
