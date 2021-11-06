// Cosmos DB Cassandra Namespace
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:cassandraKeyspace}*/cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  parent: cosmosDBAccount
  name: /*${3:'name'}*/'name'
  properties: {
    resource: {
      id: /*${4:'id'}*/'id'
    }
    options: {
      throughput: /*${5:'throughput'}*/'throughput'
    }
  }
}
