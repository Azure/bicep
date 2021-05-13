// Media Services account
resource ${1:mediaServices} 'Microsoft.Media/mediaServices@2020-05-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    storageAccounts: [
      {
        id: resourceId('Microsoft.Storage/storageAccounts', ${3:'mediaServiceStorageAccount'})
        type: '${4|Primary,Secondary|}'
      }
    ]
  }
}
