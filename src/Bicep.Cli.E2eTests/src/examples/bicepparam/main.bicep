param foo string

resource sa 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: foo
  location: 'West US'
  sku: {
    name: 'Standard_ZRS'
  }
  kind: 'StorageV2'
}
