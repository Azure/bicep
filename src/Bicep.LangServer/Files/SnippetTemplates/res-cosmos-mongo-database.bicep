// Cosmos DB Mongo Database
resource /*${1:mongoDb}*/mongoDb 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2021-06-15' = {
  name: /*${2:'name'}*/'account-name/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {}
  }
}
