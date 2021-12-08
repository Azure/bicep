// Cosmos DB Database Account
resource /*${1:cosmosDbAccount}*/cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  kind: /*${4|'GlobalDocumentDB','MongoDB','Parse'|}*/'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: /*${5|'Eventual','Session','BoundedStaleness','Strong','ConsistentPrefix'|}*/'Eventual'
      maxStalenessPrefix: /*${6:1}*/1
      maxIntervalInSeconds: /*${7:5}*/5
    }
    locations: [
      {
        locationName: /*${3:location}*/'location'
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
