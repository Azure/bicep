// $1 = mediaServices
// $2 = 'name'
// $3 = 'storageAccount.id'
// $4 = Primary

resource mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    storageAccounts: [
      {
        id: 'storageAccount.id'
        type: 'Primary'
      }
    ]
  }
}
// Insert snippet here

