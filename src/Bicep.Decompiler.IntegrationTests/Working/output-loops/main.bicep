param storageCount int = 2

var baseName_var = 'storage${uniqueString(resourceGroup().id)}'

resource baseName 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: concat(i, baseName_var)
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {}
}]

output storageEndpoints array = [for i in range(0, storageCount): reference(resourceId('Microsoft.Storage/storageAccounts', concat(i, baseName_var))).primaryEndpoints.blob]
output copyIndex array = [for i in range(0, storageCount): i]
output copyIndexWithInt array = [for i in range(0, storageCount): (i + 123)]
