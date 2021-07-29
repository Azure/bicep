// Media Services account
resource /*${1:storageAccount}*/storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: /*${2:'name'}*/'name'
}

resource /*${3:mediaServices}*/mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: /*${4:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    storageAccounts: [
      {
        id: storageAccount.id
        type: /*'${5|Primary,Secondary|}'*/'Primary'
      }
    ]
  }
}
