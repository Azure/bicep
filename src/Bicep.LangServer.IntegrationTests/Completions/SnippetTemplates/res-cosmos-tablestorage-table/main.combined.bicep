resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts/apis/tables@2016-03-31' = {
  name: 'accountName/table/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 'throughput'
    }
  }
}

