param accountName string

param location string = resourceGroup().location

resource dataLake 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: accountName
  location: location
  properties: {
    newTier: 'Consumption'
    encryptionState: 'Enabled'
    encryptionConfig: {
      'type': 'ServiceManaged'
    }
  }
}
