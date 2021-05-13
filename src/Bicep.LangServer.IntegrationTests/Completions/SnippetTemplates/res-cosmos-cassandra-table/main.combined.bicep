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

