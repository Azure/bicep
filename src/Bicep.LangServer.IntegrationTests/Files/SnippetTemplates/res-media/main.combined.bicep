// $1 = mediaServices
// $2 = 'name'
// $3 = location
// $4 = 'storageAccount.id'
// $5 = Primary

param location string

resource mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: 'name'
  location: location
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

