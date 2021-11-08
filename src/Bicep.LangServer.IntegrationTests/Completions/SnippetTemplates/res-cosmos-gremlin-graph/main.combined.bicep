// $1 = 'accountName/databaseName'
// $2 = 'id'
// $3 = 1000
// $4 = cosmosDBGremlinGraph
// $5 = 'name'
// $6 = 'id'
// $7 = 'paths'
// $8 = Hash
// $9 = consistent
// $10 = 'path'
// $11 = Hash
// $12 = String
// $13 = -1
// $14 = 'path'
// $15 = 1000

resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases@2021-06-15' = {
  name: 'accountName/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 1000
    }
  }
}

resource cosmosDBGremlinGraph 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases/graphs@2021-06-15' = {
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
        indexingMode: 'consistent'
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
      throughput: 1000
    }
  }
}
// Insert snippet here

