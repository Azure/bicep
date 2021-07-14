// Cosmos DB Mongo Database
resource /*${1:mongoDb}*/mongoDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: /*${2:'name'}*/'account-name/mongodb/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {}
  }
}
