// $1 = cassandraKeyspace
// $2 = 'accountName/databaseName'
// $3 = 'id'
// $4 = 1000

resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  name: 'accountName/databaseName'
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

