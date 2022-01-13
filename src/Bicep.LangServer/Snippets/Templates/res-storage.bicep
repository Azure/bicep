// Storage Account
resource /*${1:storageaccount}*/storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  kind: /*${4|'StorageV2','Storage','BlobStorage','BlockBlobStorage','FileStorage'|}*/'StorageV2'
  sku: {
    name: /*${5:'Premium_LRS'}*/'Premium_LRS'
  }
}
