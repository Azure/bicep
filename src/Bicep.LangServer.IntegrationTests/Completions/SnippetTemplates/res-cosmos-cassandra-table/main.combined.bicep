// $1 = 'accountName/databaseName'
// $2 = 'id'
// $3 = cassandraKeyspaceTable
// $4 = 'name'
// $5 = 'id'

resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  name: 'accountName/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}

resource cassandraKeyspaceTable 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces/tables@2021-06-15' = {
  parent: cassandraKeyspace
  name: 'name'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}
// Insert snippet here

