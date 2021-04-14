// Data Lake Store Account
resource dataLakeStore 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: '${1:dataLakeStore}'
  location: resourceGroup().location
  properties: {
    newTier: '${2|Consumption,Commitment_1TB,Commitment_10TB,Commitment_100TB,Commitment_500TB,Commitment_1PB,Commitment_5PB|}'
    encryptionState: '${3|Enabled,Disabled|}'
  }
}