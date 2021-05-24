resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: 'accountName/gremlin/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 'throughput'
    }
  }
}

resource cosmosDBGremlinGraph 'Microsoft.DocumentDb/databaseAccounts/apis/databases/graphs@2016-03-31' = {
  parent: gremlinDb
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
    options: {
      throughput: 'throughput'
    }
  }
}

