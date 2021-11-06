// Cosmos DB Cassandra Table
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'name'
}

resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  parent: cosmosDBAccount
  name: /*${2:'name'}*/'name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {}
  }
}

resource /*${4:cassandraKeyspaceTable}*/cassandraKeyspaceTable 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces/tables@2021-06-15' = {
  parent: cassandraKeyspace
  name: /*${5:'name'}*/'name'
  properties: {
    resource: {
      id: /*${6:'id'}*/'id'
    }
    options: {}
  }
}
