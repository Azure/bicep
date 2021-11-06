// $1 = 'accountName'
// $2 = mongoDb
// $3 = 'databaseName'
// $4 = 'id'

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: 'accountName'
}

resource mongoDb 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: 'databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}
// Insert snippet here

