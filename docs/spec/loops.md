# Loops

>Loops were implemented in v0.3

Loops may be used to iterate over an array to declare multiple resources or to set an array property inside a resource declaration. Iteration over the array occurs over the elements of the array. The index of the iteration is also available.

A new scope is created inside the loop body. Identifiers declared in the outer scope may be accessed inside the inner scope, but identifiers declared in the inner scope will not be added to the outer scope. [Resources](./resources.md), [variables](./variables.md), and [parameters](./parameters.md) declared at the scope of the file may be referenced within the loop body. Multiple loops may be nested inside each other.

Filtering the loop is also allowed via the `where` keyword. (See the examples below for more details.)

## Examples

### Declare multiple identical resources
In the below example, we are looping over `storageAccounts` array. For each loop iteration, `storageName` is set to the current array item and is referenced by name in the loop body.
```
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
  kind: StorageV2
  sku: {
    name: 'Standard_LRS'
  }
}]
```

### Use the loop index
In the example below, we are iterating over the `storageConfiguration` array variable. Within the loop body, `config` stores the current element from the array and `i` stores the 0-based index of the current array element. Both are referenced from within the loop body.

```
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
  name: storageAccountNamePrefix + config.suffix + i
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
  kind: StorageV2
  sku: {
    name: config.sku
  }
}]

```

### Generate an array property using a loop
In the example below, we are constructing a `subnets` property of a virtual network resource from the `subnets` array. On each loop iteration, the `subnet` variable is set to the current element of the array.

```
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
    },
    subnets: [for subnet in subnets: {
      name: subnet.name
      properties: {
        addressPrefix: subnet.subnetPrefix
      }
    }]
  }
}
```

### Nested loops and filtering.
The example below demonstrates a nested loop combined with filters at each loop. Filters must be expressions that evaluate to a boolean value.

```
resource parentResources 'Microsoft.Example/examples@2020-06-06' = [for parent in parents where parent.enabled: {
  name: parent.name
  properties: {
    children: [for child in parent.children where parent.includeChildren && child.enabled: {
      name: child.name
      setting: child.settingValue
    }]
  }
}]
```
