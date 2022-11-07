// Media Services account
resource /*${1:mediaServices}*/mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    storageAccounts: [
      {
        id: /*${4:'storageAccount.id'}*/'storageAccount.id'
        type: /*'${5|Primary,Secondary|}'*/'Primary'
      }
    ]
  }
}
