# Convert any* ARM Template into a Bicep file

**assumes that the ARM template only uses [syntax that is supported](../spec) in Bicep*

Since Bicep is a transparent abstraction of ARM templates, any resource that can be deployed via an ARM template can be authored in Bicep. However, not all ARM template capabilities are supported in Bicep in the 0.1 release. The following statements must be true in order for the template to be convertible:

* Template does *not* use the `copy` function for creating multiple resources, multiple variables, or multiple outputs
* Template does *not* conditionally deploy resources with the `condition` property
* Template does not deploy across scopes (though this can be hacked together by using the `Microsoft.Resources/deployments` resource and using the `templateLink` or `template` property to insert the full ARM template)


## Convert parameters, variables, and outputs

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

## Convert an entire ARM template

You can take a look at an entire sample template converted to a bicep file here:

[ARM Template](./complete-bicep-files/05.json) -> [Bicep file](./complete-bicep-files/05.bicep)

We also have a growing set of [examples](../examples) of fully converted [Azure QuickStart templates](https://github.com/Azure/azure-quickstart-templates)
