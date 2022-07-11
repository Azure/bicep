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
  name: cosmosDBDatabaseName // "The property \"name\" is declared multiple times in this object. Remove or rename the duplicate properties.","
  properties: {
                // The property "properties" is declared multiple times in this object. Remove or rename the duplicate properties.
    resource: {
      id: cosmosDBDatabaseName
    }
    options: {
      throughput: cosmosDBDatabaseThroughput
    }
  }
}
