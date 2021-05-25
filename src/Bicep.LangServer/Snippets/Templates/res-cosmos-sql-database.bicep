// Cosmos DB SQL Database
resource ${1:sqlDb} 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: ${2:'name'}
  properties: {
    resource: {
      id: ${3:'id'}
    }
    options: {
      throughput: ${4:'throughput'}
    }
  }
}
