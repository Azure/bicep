// $1 = 'accountName'
// $2 = gremlinDb
// $3 = 'databaseName'
// $4 = 'id'
// $5 = 1000

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: 'accountName'
}

resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: 'databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 1000
    }
  }
}
// Insert snippet here

