// Cosmos Table Storage
resource ${1:databaseAccount} 'Microsoft.DocumentDB/databaseAccounts/apis/tables@2016-03-31' = {
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
