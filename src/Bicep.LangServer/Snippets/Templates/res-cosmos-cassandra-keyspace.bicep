// Cosmos DB Cassandra Namespace
resource ${1:cassandraKeyspace} 'Microsoft.DocumentDB/databaseAccounts/apis/keyspaces@2016-03-31' = {
  name: ${2:'name'}
  properties: {
    resource: {
      id: ${3:'id'}
    }
    options: {
      throughput: ${4:'throughput'}
    }
  }
}
