// $1 = 'accountName'
// $2 = 'keyspaceName'
// $3 = 'id'
// $4 = cassandraKeyspaceTable
// $5 = 'name'
// $6 = 'id'

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: 'accountName'
}

resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  parent: cosmosDBAccount
  name: 'keyspaceName'
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

