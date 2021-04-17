resource dataLakeStore 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: 'dataLakeStore'
  location: resourceGroup().location
  properties: {
    newTier: 'Consumption'
    encryptionState: 'Enabled'
  }
}
