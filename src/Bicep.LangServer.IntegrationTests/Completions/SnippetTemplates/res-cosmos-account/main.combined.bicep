// $1 = cosmosDbAccount
// $2 = 'name'
// $3 = 'GlobalDocumentDB'
// $4 = 'Eventual'
// $5 = 1
// $6 = 5
// $7 = 'location'
// $8 = 0
// $9 = true
// $10 = 'EnableTable'

param location string
//@[6:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'name'
  location: 'location'
//@[12:22) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'location' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'location'|
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Eventual'
      maxStalenessPrefix: 1
      maxIntervalInSeconds: 5
    }
    locations: [
      {
        locationName: 'location'
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

