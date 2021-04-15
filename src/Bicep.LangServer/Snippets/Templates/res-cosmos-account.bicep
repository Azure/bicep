﻿// Cosmos DB Database Account
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: ${1:cosmosDbAccount}
  location: resourceGroup().location
  kind: ${2|GlobalDocumentDB,MongoDB,Parse|}
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: ${3|Eventual,Session,BoundedStaleness,Strong,ConsistentPrefix|}
      maxStalenessPrefix: ${4:1}
      maxIntervalInSeconds: ${5:5}
    }
    locations: [
      {
        locationName: ${6:testLocation}
        failoverPriority: ${7:0}
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: ${9|true,false|}
    capabilities: [
      {
        name: ${10|EnableTable,EnableGremlin|}
      }
    ]
  }
}
