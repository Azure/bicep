// $1 = 'accountName'
// $2 = cosmosTable
// $3 = 'databaseName'
// $4 = 'id'
// $5 = 100
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: 'accountName'
}

resource cosmosTable 'Microsoft.DocumentDB/databaseAccounts/tables@2021-06-15' = {
  parent: cosmosDBAccount
  name: 'databaseName'
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

