// $1 = cosmosDbAccount
// $2 = 'name'
// $3 = location
// $4 = 'GlobalDocumentDB'
// $5 = 'Eventual'
// $6 = 1
// $7 = 5
// $8 = 0
// $9 = true
// $10 = 'EnableTable'

param location string

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'name'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Eventual'
      maxStalenessPrefix: 1
      maxIntervalInSeconds: 5
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: true
    capabilities: [
      {
        name: 'EnableTable'
      }
    ]
  }
}
// Insert snippet here

