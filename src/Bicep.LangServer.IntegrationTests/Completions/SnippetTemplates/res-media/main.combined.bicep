resource mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    storageAccounts: [
      {
        id: resourceId('Microsoft.Storage/storageAccounts', 'mediaServiceStorageAccount')
        type: 'Primary'
      }
    ]
  }
}

