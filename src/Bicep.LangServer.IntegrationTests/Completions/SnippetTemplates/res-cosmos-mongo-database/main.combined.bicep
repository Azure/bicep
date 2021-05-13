resource mongoDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: 'accountName/mongodb/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}

