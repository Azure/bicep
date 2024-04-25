resource foo 'Microsoft.Storage/storageAccounts@2021-01-01' = {
  name: 'foo'
  kind: 'StorageV2'
  location: ''
  sku: {
    name: 'Standard_RAGRS'
  }
}

