// Cosmos Table Storage
resource /*${1:cosmosTable}*/cosmosTable 'Microsoft.DocumentDB/databaseAccounts/apis/tables@2016-03-31' = {
  name: /*${2:'name'}*/'account-name/table/database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {
      throughput: /*${4:'throughput'}*/'throughput'
    }
  }
}
