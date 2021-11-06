// Cosmos DB Gremlin Database
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:gremlinDb}*/gremlinDb 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: /*${3:'name'}*/'database-name'
  properties: {
    resource: {
      id: /*${4:'id'}*/'id'
    }
    options: {
      throughput: /*${5:'throughput'}*/'throughput'
    }
  }
}
