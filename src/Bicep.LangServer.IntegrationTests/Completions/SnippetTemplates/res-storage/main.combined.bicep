// $1 = storageaccount
// $2 = 'name'
// $3 = 'StorageV2'
// $4 = 'Premium_LRS'

resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}
// Insert snippet here

