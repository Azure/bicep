// $1 = gremlinDb
// $2 = 'accountName/gremlin/databaseName'
// $3 = 'id'
// $4 = 'throughput'

resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: 'accountName/gremlin/databaseName'
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

