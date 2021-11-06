// Cosmos DB SQL Database
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'account-name'
}

resource /*${2:sqlDb}*/sqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: /*${3:'name'}*/'database-name'
  properties: {
    resource: {
      id: /*${4:'id'}*/'id'
    }
    options: {
      throughput: /*${5:'throughput'}*/'throughput'
    }
  }
}
