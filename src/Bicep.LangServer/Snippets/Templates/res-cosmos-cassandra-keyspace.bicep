// Cosmos DB Cassandra Namespace
resource /*${1:cassandraKeyspace}*/cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/apis/keyspaces@2016-03-31' = {
  name: /*${2:'name'}*/'account-name/cassandra/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {
      throughput: /*${4:'throughput'}*/'throughput'
    }
  }
}
