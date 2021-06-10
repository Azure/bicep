// $1 = cosmosTable
// $2 = 'accountName/table/databaseName'
// $3 = 'id'
// $4 = 'throughput'

resource cosmosTable 'Microsoft.DocumentDB/databaseAccounts/apis/tables@2016-03-31' = {
  name: 'accountName/table/databaseName'
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

