// $1 = 'accountName'
// $2 = 'databaseName'
// $3 = 'id'
// $4 = 1000
// $5 = cosmosDBGremlinGraph
// $6 = 'name'
// $7 = 'id'
// $8 = 'paths'
// $9 = Hash
// $10 = consistent
// $11 = 'path'
// $12 = Hash
// $13 = String
// $14 = -1
// $15 = 'path'
// $16 = 1000

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: 'accountName'
}

resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: 'databaseName'
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

