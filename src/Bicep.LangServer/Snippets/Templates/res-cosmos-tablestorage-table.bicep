// Cosmos Table Storage
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'account-name'
}

resource /*${2:cosmosTable}*/cosmosTable 'Microsoft.DocumentDB/databaseAccounts/tables@2021-06-15' = {
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
