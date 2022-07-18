param cosmosDBDatabaseName string = 'db1'
param cosmosDBDatabaseThroughput int = 4000
param location string = 'westus'

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: uniqueString(resourceGroup().id)
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

resource cosmosDBDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2020-04-01' = {
  parent: cosmosDBAccount
  name: cosmosDBDatabaseName
  properties: {
    // test comment formatting
    resource: {
      id: cosmosDBDatabaseName
    }
    options: {
      throughput: cosmosDBDatabaseThroughput
    }
  }
}