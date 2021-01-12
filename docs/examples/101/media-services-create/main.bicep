param mediaServiceName string = 'mediaService${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location

var storageAccountName = 'storage${uniqueString(resourceGroup().id)}'

resource mediaService 'Microsoft.Media/mediaservices@2020-05-01' = {
  name: mediaServiceName
  location: location
  properties: {
    storageAccounts: [
      {
        id: storageAccount.id
        type: 'Primary'
      }
    ]
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
