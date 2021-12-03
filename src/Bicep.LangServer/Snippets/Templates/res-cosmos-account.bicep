// Cosmos DB Database Account
resource /*${1:cosmosDbAccount}*/cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: /*${2:'name'}*/'name'
  location: /*${7:location}*/'location'
  kind: /*${3|'GlobalDocumentDB','MongoDB','Parse'|}*/'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: /*${4|'Eventual','Session','BoundedStaleness','Strong','ConsistentPrefix'|}*/'Eventual'
      maxStalenessPrefix: /*${5:1}*/1
      maxIntervalInSeconds: /*${6:5}*/5
    }
    locations: [
      {
        locationName: /*${7:location}*/'location'
        failoverPriority: /*${8:0}*/0
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: /*${9|true,false|}*/true
    capabilities: [
      {
        name: /*${10|'EnableTable','EnableGremlin'|}*/'EnableTable'
      }
    ]
  }
}
