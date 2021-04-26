// Cosmos DB Database Account
resource ${1:cosmosDbAccount} 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: ${2:'name'}
  location: resourceGroup().location
  kind: ${3|'GlobalDocumentDB','MongoDB','Parse'|}
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: ${4|'Eventual','Session','BoundedStaleness','Strong','ConsistentPrefix'|}
      maxStalenessPrefix: ${5:1}
      maxIntervalInSeconds: ${6:5}
    }
    locations: [
      {
        locationName: ${7:'location'}
        failoverPriority: ${8:0}
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: ${9|true,false|}
    capabilities: [
      {
        name: ${10|'EnableTable','EnableGremlin'|}
      }
    ]
  }
}
