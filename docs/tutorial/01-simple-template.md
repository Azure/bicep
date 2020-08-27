# Working with a basic bicep file

In this tutorial we'll start from a blank file and build up to a file with the basic bicep primitives.

If you haven't already, follow [these instructions](../installing.md) to install the bicep CLI and VS Code extension.

## Compile an empty bicep file

Let's start by creating a blank file `main.bicep` and compiling it by running:

```bash
bicep build main.bicep
```

You should get an output json file of the same name in your current directory -- in this case `main.json`. It should be a skeleton ARM JSON template:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {},
  "functions": [],
  "variables": {},
  "resources": [],
  "outputs": {}
}
```

## Add a resource

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

When we compile the template with `bicep build main.bicep`, we see the following JSON:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {},
  "functions": [],
  "variables": {},
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "uniquestorage001",
      "location": "eastus",
      "kind": "Storage",
      "sku": {
        "name": "Standard_LRS"
      }
    }
  ],
  "outputs": {}
}
```

At this point, I can deploy it like any other ARM template using the standard command line tools (`az deployment group create ...` or `New-AzResourceGroupDeployment ...`).

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

The end of the parameter declaration (`= 'eastus'`) is only the *default* value. It can be optionally overridden at deployment time.

Let's compile with `bicep build main.bicep` and look at the output:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": {
      "type": "string",
      "defaultValue": "eastus"
    },
    "name": {
      "type": "string",
      "defaultValue": "uniquestorage001"
    }
  },
  "functions": [],
  "variables": {},
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[parameters('name')]",
      "location": "[parameters('location')]",
      "kind": "Storage",
      "sku": {
        "name": "Standard_LRS"
      }
    }
  ],
  "outputs": {}
}
```

## Add variables and outputs

I can also add `variables` for storing values or complex expressions, and emit `outputs` to be passed to a script or another template:

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

Notice I can easily reference the resource `Id` from the symbolic name of the storage account (`stg.id`) which we will translate to the `resourceId(...)` function in the compiled template. When compiled, you should see the following ARM Template JSON:

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": {
      "type": "string",
      "defaultValue": "eastus"
    },
    "name": {
      "type": "string",
      "defaultValue": "uniquestorage001"
    }
  },
  "functions": [],
  "variables": {
    "storageSku": "Standard_LRS"
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[parameters('name')]",
      "location": "[parameters('location')]",
      "kind": "Storage",
      "sku": {
        "name": "[variables('storageSku')]"
      }
    }
  ],
  "outputs": {
    "storageId": {
      "type": "string",
      "value": "[resourceId('Microsoft.Storage/storageAccounts', parameters('name'))]"
    }
  }
}
```

## Next steps

In the next tutorial, we will walk through compiling and deploying a bicep file:

[2 - Deploying a bicep file](./02-deploying-a-bicep-file.md)
