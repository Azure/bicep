resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2019-12-12' = {
  name: 'testCosmosDbAccount'
  location: resourceGroup().location
  kind: 'MongoDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Eventual'
      maxStalenessPrefix: 1
      maxIntervalInSeconds: 5
    }
    locations: [
      {
        locationName: 'testLocation'
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    ipRangeFilter: 'testIpRangeFilter'
    enableAutomaticFailover: true
    capabilities: [
      {
        name: 'EnableTable'
      }
    ]
  }
}
