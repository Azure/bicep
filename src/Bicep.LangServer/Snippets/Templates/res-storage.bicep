// Storage Account
resource ${1:'storageaccount'} 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  kind: ${3|'StorageV2','Storage','BlobStorage','BlockBlobStorage','FileStorage'|}
  sku: {
    name: ${4:'Premium_LRS'}
    tier: ${5|'Premium','Standard'|}
  }
}
