// Cosmos DB Gremlin Database
resource ${1:databaseAccount} 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: ${2:'accountName/gremlin/databaseName'}
  properties: {
    resource: {
      id: ${3:'id'}
    }
    options: {
      throughput: ${4:'throughput'}
    }
  }
}
