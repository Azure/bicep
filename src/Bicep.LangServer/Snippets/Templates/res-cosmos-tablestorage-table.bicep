// Cosmos Table Storage
resource /*${1:cosmosTable}*/cosmosTable 'Microsoft.DocumentDB/databaseAccounts/tables@2021-06-15' = {
  name: /*${2:'name'}*/'account-name/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {
      throughput: /*${4:'throughput'}*/'throughput'
    }
  }
}
