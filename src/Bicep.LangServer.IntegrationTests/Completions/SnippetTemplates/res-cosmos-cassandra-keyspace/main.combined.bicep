// $1 = cassandraKeyspace
// $2 = 'accountName/cassandra/databaseName'
// $3 = 'id'
// $4 = 'throughput'

resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/apis/keyspaces@2016-03-31' = {
  name: 'accountName/cassandra/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 'throughput'
    }
  }
}
// Insert snippet here

