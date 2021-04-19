// Data Lake Store Account
resource ${1:'dataLakeStore'} 'Microsoft.DataLakeStore/accounts@2016-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    newTier: '${2|Consumption,Commitment_1TB,Commitment_10TB,Commitment_100TB,Commitment_500TB,Commitment_1PB,Commitment_5PB|}'
    encryptionState: '${3|Enabled,Disabled|}'
  }
}
