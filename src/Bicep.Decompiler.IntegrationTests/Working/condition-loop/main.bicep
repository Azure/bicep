param storageCount int = 2
param storagePrefix string

resource storagePrefix_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): if (i == 1) {
  name: toLower(concat(i, storagePrefix, uniqueString(resourceGroup().id)))
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {}
}]
