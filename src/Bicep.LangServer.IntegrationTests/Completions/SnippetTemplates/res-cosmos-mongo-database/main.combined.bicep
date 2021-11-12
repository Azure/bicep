// $1 = mongoDb
// $2 = 'accountName/databaseName'
// $3 = 'id'

resource mongoDb 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2021-06-15' = {
  name: 'accountName/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}
// Insert snippet here

