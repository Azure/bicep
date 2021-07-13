// Media Services account
resource /*${1:mediaServices}*/mediaServices 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    storageAccounts: [
      {
        id: resourceId('Microsoft.Storage/storageAccounts', /*${3:'mediaServiceStorageAccount'}*/'mediaServiceStorageAccount')
        type: /*'${4|Primary,Secondary|}'*/'Primary'
      }
    ]
  }
}
