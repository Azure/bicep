param name string

resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: name
  #disable-next-line no-loc-expr-outside-params
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}

output blobEndpoint string = storage.properties.primaryEndpoints.blob
