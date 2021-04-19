// Cosmos DB Database Account
resource ${1:'cosmosDbAccount'} 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'name'
  location: resourceGroup().location
  kind: '${2|GlobalDocumentDB,MongoDB,Parse|}'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: '${3|Eventual,Session,BoundedStaleness,Strong,ConsistentPrefix|}'
      maxStalenessPrefix: ${4:1}
      maxIntervalInSeconds: ${5:5}
    }
    locations: [
      {
        locationName: ${6:'location'}
        failoverPriority: ${7:0}
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: ${8|true,false|}
    capabilities: [
      {
        name: '${9|EnableTable,EnableGremlin|}'
      }
    ]
  }
}
