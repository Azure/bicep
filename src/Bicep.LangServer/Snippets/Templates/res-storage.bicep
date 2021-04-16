// Storage Account
resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: ${1:storageaccount}
  location: resourceGroup().location
  kind: ${2|StorageV2,Storage,BlobStorage,BlockBlobStorage,FileStorage|}
  sku: {
    name: ${3:Premium_LRS}
    tier: ${4|Premium,Standard|}
  }
}
