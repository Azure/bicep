// $1 = dataLakeStore
// $2 = 'name'
// $3 = 'Consumption'
// $4 = 'Enabled'

resource dataLakeStore 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    newTier: 'Consumption'
    encryptionState: 'Enabled'
  }
}
// Insert snippet here

