# Expressions
> **Note**: Expressions are not implemented yet. 

Other content TBD. This section will be expanded as we design the expression syntax and semantics.

## Ternary operator
The ternary operator evaluates a `<condition>` on the left side of the `?`, and
 either evaluates and returns the left-hand `<valueA>` or right-hand `<valueB>` of the expressions separated by `:` on the right side of the `?`.

The structure of the ternary operator is as follows:
```
<condition> ? <valueA> : <valueB>
```

Example usage:
```
parameter replicateGlobally bool

resource myStorageAccount `Microsoft.Storage/storageAccounts@2017-10-01` = {
  name: storageAccountName
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
    name: replicateGlobally ? 'Standard_GRS' : 'Standard_LRS'
  }
}
```