// Cosmos DB Cassandra Namespace
resource /*${1:cassandraKeyspace}*/cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/cassandraKeyspaces@2021-06-15' = {
  name: /*${2:'name'}*/'account-name/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {
      throughput: /*${4:'throughput'}*/'throughput'
    }
  }
}
