// Media Services account
resource /*${1:mediaServices}*/mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    storageAccounts: [
      {
        id: /*${3:'storageAccount.id'}*/'storageAccount.id'
        type: /*'${4|Primary,Secondary|}'*/'Primary'
      }
    ]
  }
}
