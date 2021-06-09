# Loops

Loops may be used to iterate over an array to declare multiple resources/modules or to set an array property inside a resource/module declaration. Loops may also be used when defining variables. Iteration over the array occurs over the elements of the array. The index of the iteration is also available.

A new scope is created inside the loop body. Identifiers declared in the outer scope may be accessed inside the inner scope, but identifiers declared in the inner scope will not be added to the outer scope. [Resources](./resources.md), [variables](./variables.md), and [parameters](./parameters.md) declared at the scope of the file may be referenced within the loop body. Multiple loops may be nested inside each other.

Filtering the loop is also allowed via the `if` keyword in the loop body. (See the examples below for more details.)

## Examples

### Declare multiple identical resources

In the below example, we are looping over `storageAccounts` array. For each loop iteration, `storageName` is set to the current array item and is referenced by name in the loop body.

```bicep
// array of storage account names
param storageAccounts array

resource storageAccountResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for storageName in storageAccounts: {
  name: storageName
  location: resourceGroup().location
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}]
```

### Use the loop index

To write a simple index-based loop you can use the `range()` function so that your iterator conceptually represents the index of the current iteration. This is most similar to ARM Template JSON loops with the `copyIndex()` function. For example, we can modify the above example to be index-based:

```bicep
resource storageAccountResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0,3): {
  name: 'storageName${i}'
  location: resourceGroup().location
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}]
```

We can also retrieve the index even when iterating through an array of objects. In the example below, we are iterating over the `storageConfiguration` array variable. Within the loop body, `config` stores the current element from the array and `i` stores the 0-based index of the current array element. Both are referenced from within the loop body.

```bicep
param storageAccountNamePrefix string

var storageConfigurations = [
  {
    suffix: 'local'
    sku: 'Standard_LRS'
  }
  {
    suffix: 'geo'
    sku: 'Standard_GRS'
  }
]

resource storageAccountResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (config, i) in storageConfigurations: {
  name: '${storageAccountNamePrefix}${config.suffix}${i}'
  location: resourceGroup().location
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: config.sku
  }
}]

```

### Generate an array property using a loop

In the example below, we are constructing a `subnets` property of a virtual network resource from the `subnets` array. On each loop iteration, the `subnet` variable is set to the current element of the array.

```bicep
var subnets = [
  {
    name: 'api'
    subnetPrefix: '10.144.0.0/24'
  }
  {
    name: 'worker'
    subnetPrefix: '10.144.1.0/24'
  }
]

resource vnet 'Microsoft.Network/virtualNetworks@2018-11-01' = {
  name: 'vnet'
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.144.0.0/20'
      ]
    }
    subnets: [for subnet in subnets: {
      name: subnet.name
      properties: {
        addressPrefix: subnet.subnetPrefix
      }
    }]
  }
}
```

### Generate an array variable using a loop

In the example below we are implementing the same loop as in the previous example, but instead defining the `subnets` property as a variable `subnets` by iterating over the `subnetsDefinitions` parameter.


```bicep
param subnetsDefinitions array = [
  {
    name: 'api'
    subnetPrefix: '10.144.0.0/24'
  }
  {
    name: 'worker'
    subnetPrefix: '10.144.1.0/24'
  }
]

var subnets = [for subnet in subnetsDefinitions: {
  name: subnet.name
  properties: {
    addressPrefix: subnet.subnetPrefix
  }
}]

resource vnet 'Microsoft.Network/virtualNetworks@2018-11-01' = {
  name: 'vnet'
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.144.0.0/20'
      ]
    }
    subnets: subnets
  }
}
```

### Nested loops and filtering

The example below demonstrates a nested loop combined with a filtered resource loop. Filters must be expressions that evaluate to a boolean value.

```bicep
resource parentResources 'Microsoft.Example/examples@2020-06-06' = [for parent in parents: if(parent.enabled) {
  name: parent.name
  properties: {
    children: [for child in parent.children: {
      name: child.name
      setting: child.settingValue
    }]
  }
}]
```

Filters are also supported with module loops.

### Batch size decorator

By default for-expressions used in values of module or resource declarations will be deployed concurrently in a non-deterministic order at runtime. This behavior can be changed with the `@batchSize` decorator. The decorator is allowed on resource or module declarations whose values are a for-expression. The decorator accepts one integer literal parameter with value equal or greater than 1.

When the decorator is specified the resources or modules part of the same declaration will be deployed sequentially in batches of the specified size. Each batch will be deployed concurrently. For purely sequential deployment, set the batch size to 1.

The following example deploys 10 resource groups 2 at a time:

```bicep
targetScope = 'subscription'

@batchSize(2)
resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for i in range(0,10): {
  name: 'my-rg-{i}'
  location: 'eastus'
}]
```

See [Serial or Parallel](https://docs.microsoft.com/azure/azure-resource-manager/templates/copy-resources#serial-or-parallel) for more information.

### Output loops

Directly referencing a resource module or module collection is not currently supported in output loops. In order to loop outputs we need to apply an array indexer to the expression.

```bicep
var nsgNames = [
  'nsg1'
  'nsg2'
  'nsg3'
]

resource nsg 'Microsoft.Network/networkSecurityGroups@2020-06-01' = [for name in nsgNames: {
  name: name
  location: resourceGroup().location
}]

output nsgs array = [for (name, i) in nsgNames: {
  name: nsg[i].name
  resourceId: nsg[i].id
}]
```
