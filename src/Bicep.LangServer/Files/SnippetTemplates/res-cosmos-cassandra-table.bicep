// Cosmos DB Cassandra Table
resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  name: /*${1:'name'}*/'account-name/database-name'
  properties: {
    resource: {
      id: /*${2:'id'}*/'id'
    }
    options: {}
  }
}

resource /*${3:cassandraKeyspaceTable}*/cassandraKeyspaceTable 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces/tables@2021-06-15' = {
  parent: cassandraKeyspace
  name: /*${4:'name'}*/'name'
  properties: {
    resource: {
      id: /*${5:'id'}*/'id'
    }
    options: {}
  }
}
