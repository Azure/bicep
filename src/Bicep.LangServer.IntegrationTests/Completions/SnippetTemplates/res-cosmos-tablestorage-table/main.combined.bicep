// $1 = cosmosTable
// $2 = 'accountName/databaseName'
// $3 = 'id'
// $4 = 100

resource cosmosTable 'Microsoft.DocumentDB/databaseAccounts/tables@2021-06-15' = {
  name: 'accountName/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 100
    }
  }
}
// Insert snippet here

