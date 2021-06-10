// $1 = 'accountName/gremlin/databaseName'
// $2 = 'id'
// $3 = 'throughput'
// $4 = cosmosDBGremlinGraph
// $5 = 'name'
// $6 = 'id'
// $7 = 'paths'
// $8 = Hash
// $9 = Consistent
// $10 = 'path'
// $11 = Hash
// $12 = String
// $13 = -1
// $14 = 'path'
// $15 = 'throughput'

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
// Insert snippet here

