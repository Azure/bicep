# Convert any* ARM Template into a Bicep file

**assumes that the ARM template only uses [syntax that is supported](../spec) in Bicep*

Since Bicep is a transparent abstraction of ARM templates, any resource that can be deployed via an ARM template can be authored in Bicep. However, not all ARM template capabilities are supported in Bicep in the 0.2 release. The following statements must be true in order for the template to be convertible:

* Template does *not* use the `copy` function for creating multiple resources, multiple variables, or multiple outputs
* Template does *not* conditionally deploy resources with the `condition` property

## Decompiling an ARM Template
> Requires Bicep CLI v0.2.59 or later
 
The Bicep CLI provides the ability to [decompile](../decompiling.md) any existing ARM Template to a `.bicep` file, using the `bicep decompile` command.

Here's an example ARM Template that deploys a storage account.

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "location": {
            "defaultValue": "[resourceGroup().location]",
            "type": "String"
        },
        "namePrefix": {
            "defaultValue": "contoso",
            "type": "String"
        },
        "production": {
            "defaultValue": false,
            "type": "bool"
        }
    },
    "variables": {
        "storageName": "[concat(parameters('nameprefix'), uniqueString(resourceGroup().id),'st')]"
    },
    "resources": [
        {
            "type": "Microsoft.Storage/storageAccounts",
            "apiVersion": "2019-06-01",
            "name": "[variables('storageName')]",
            "location": "[parameters('location')]",
            "kind": "Storage",
            "sku": {
                "name": "[if(parameters('production'), 'Standard_ZRS', 'Standard_LRS')]"
            }
        }
    ]
}
```

Let's decompile the ARM template with `bicep decompile ".\storage.json"` and look at the output `.bicep` file.

```
param location string = resourceGroup().location
param namePrefix string = 'contoso'
param production bool = false

var storageName_var = '${namePrefix}${uniqueString(resourceGroup().id)}st'

resource storageName 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageName_var
  location: location
  kind: 'Storage'
  sku: {
    name: (production ? 'Standard_ZRS' : 'Standard_LRS')
  }
}
```

Now we have a good starting point for continued Bicep authoring. Note that the variable `storagename` have changed to `storageName_var` and the symbolic name for the storage account resource is set to `storagename` based on the variable name used for the storage name in the ARM Template.

> Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep. You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible. If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.

## Manually convert parameters, variables, and outputs

Parameter, variable and output declarations are relatively simple to convert. Let's look at some code samples and convert them into Bicep syntax:

### Convert a parameter

To convert a simple parameter (often they just have a type and default value):

ARM Template:
```json
...
    "parameters": {
        "name": {
            "type": "string",
            "defaultValue": "myName"
        }
    },
...
```

Bicep:
```
param name string = 'myName'
```

For a more complex parameter with modifiers such as `allowed`:

ARM Template:
```json
...
    "parameters": {
        "name": {
            "type": "string",
            "defaultValue": "myName",
            "allowed": [
                "myName",
                "myOtherName"
            ],
            "minLength": 3,
            "maxLength": 24
        }
    },
...
```

Bicep:

```
param name string {
  default: 'myName'
  allowed: [
    'myName'
    'myOtherName'
  ]
  minLength: 3
  maxLength: 24
}
```

### Convert a variable

Translation of a simple string variable:

ARM Template:

```
"variables": {
    "location": "eastus"
}
```

Bicep:

```
var location = 'eastus'
```

### Convert an output

Translation of a simple string output

ARM Template:

```
"outputs": {
    "myOutput": {
        "type": "string",
        "value": "my output value"
    }
}
```

Bicep:

```
output myOutput string = 'my output value'
```

## Convert resources

Let's take a basic resource declared in ARM, and convert it to Bicep. Notice that all properties required in the ARM template are still required in bicep:

ARM Template:

```
"resources": [
    {
        "apiVersion": "2018-10-01",
        "type": "Microsoft.Network/virtualNetworks",
        "name": "vnet001",
        "location": "[resourceGroup().location]",
        "tags": {
            "CostCenter": "12345",
            "Owner": "alex"
        },
        "properties": {
            "addressSpace": {
                "addressPrefixes": [
                    "10.0.0.0/15"
                ]
            },
            "enableVmProtection": false,
            "enableDdosProtection": false,
            "subnets": [
                {
                    "name": "subnet001",
                    "properties": {
                        "addressPrefix": "10.0.0.0/24"
                    }
                },
                {
                    "name": "subnet002",
                    "properties": {
                        "addressPrefix": "10.0.1.0/24"
                    }
                }
            ]
        }
    }
]
```

Bicep:

```
resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
  name: 'vnet001'
  location: resourceGroup().location
  tags: {
    CostCenter: '12345'
    Owner: 'alex'
  }
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/15'
      ]
    }
    enableVmProtection: false
    enableDdosProtection: false
    subnets: [
      {
        name: 'subnet001'
        properties: {
          addressPrefix: '10.0.0.0/24'
        }
      }
      {
        name: 'subnet002'
        properties: {
          addressPrefix: '10.0.1.0/24'
        }
      }
    ]
  }
}
```

## Sample templates

You can take a look at an entire sample template converted to a bicep file here:

[ARM Template](./complete-bicep-files/05.json) -> [Bicep file](./complete-bicep-files/05.bicep)

We also have a growing set of [examples](../examples) of fully converted [Azure QuickStart templates](https://github.com/Azure/azure-quickstart-templates).
