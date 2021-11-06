// $1 = 'accountName'
// $2 = cassandraKeyspace
// $3 = 'keyspaceName'
// $4 = 'id'
// $5 = 1000

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
    options: {
      throughput: 1000
    }
  }
}
// Insert snippet here

