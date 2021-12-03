// $1 = dataLakeStore
// $2 = 'name'
// $3 = 'Consumption'
// $4 = 'Enabled'

param location string

resource dataLakeStore 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: 'name'
  location: location
  properties: {
    newTier: 'Consumption'
    encryptionState: 'Enabled'
  }
}
// Insert snippet here

