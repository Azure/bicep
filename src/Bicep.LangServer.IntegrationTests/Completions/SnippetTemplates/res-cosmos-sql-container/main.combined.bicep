resource sqlContainerName 'Microsoft.DocumentDb/databaseAccounts/apis/databases/containers@2016-03-31' = {
  parent: databaseAccount 
  name: 'name'
  properties: {
    resource: {
      id: 'id'
      partitionKey: {
        paths: [
          'paths'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'Consistent'
        includedPaths: [
          {
            path: 'path'
            indexes: [
              {
                kind: 'Hash'
                dataType: 'String'
                precision: -1
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: 'path'
          }
        ]
      }
    }
    options: {}
  }
}

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: 'accountName/sql/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}

