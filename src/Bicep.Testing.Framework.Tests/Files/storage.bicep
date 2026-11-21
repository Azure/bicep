param allowPublicAccess bool

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'teststorage'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: allowPublicAccess
    minimumTlsVersion: 'TLS1_2'
  }
}

output storageId string = storage.id
