// $1 = storageAccount
// $2 = 'name'
// $3 = mediaServices
// $4 = 'name'
// $5 = Primary

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: 'name'
}

resource mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    storageAccounts: [
      {
        id: storageAccount.id
        type: 'Primary'
      }
    ]
  }
}
// Insert snippet here
