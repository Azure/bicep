resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'testStorageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
    tier: 'Premium'
  }
}

