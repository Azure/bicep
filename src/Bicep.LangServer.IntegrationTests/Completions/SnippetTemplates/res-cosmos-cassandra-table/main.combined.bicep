// $1 = 'accountName/cassandra/databaseName'
// $2 = 'id'
// $3 = cassandraKeyspaceTable
// $4 = 'name'
// $5 = 'id'

resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/apis/keyspaces@2016-03-31' = {
  name: 'accountName/cassandra/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}

resource cassandraKeyspaceTable 'Microsoft.DocumentDb/databaseAccounts/apis/keyspaces/tables@2016-03-31' = {
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

