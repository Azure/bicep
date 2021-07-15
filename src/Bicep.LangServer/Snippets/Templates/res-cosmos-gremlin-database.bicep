// Cosmos DB Gremlin Database
resource /*${1:gremlinDb}*/gremlinDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: /*${2:'name'}*/'account-name/gremlin/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {
      throughput: /*${4:'throughput'}*/'throughput'
    }
  }
}
