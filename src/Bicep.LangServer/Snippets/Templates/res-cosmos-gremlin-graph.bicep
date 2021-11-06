// Cosmos DB Gremlin Graph
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'name'
}

resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: /*${2:'name'}*/'database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {
      throughput: /*${4:'throughput'}*/'throughput'
    }
  }
}

resource /*${5:cosmosDbGremlinGraph}*/cosmosDbGremlinGraph 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases/graphs@2021-06-15' = {
  parent: gremlinDb
  name: /*${6:'name'}*/'name'
  properties: {
    resource: {
      id: /*${7:'id'}*/'id'
      partitionKey: {
        paths: [
          /*${8:'paths'}*/'paths'
        ]
        kind: /*'${9|Hash,Range|}'*/'Hash'
      }
      indexingPolicy: {
        indexingMode: /*'${10|consistent,lazy,none|}'*/'consistent'
        includedPaths: [
          {
            path: /*${11:'path'}*/'path'
            indexes: [
              {
                kind: /*'${12|Hash,Range,Spatial|}'*/'Hash'
                dataType: /*'${13|String,Number,Point,Polygon,LineString,MultiPolygon|}'*/'String'
                precision: /*${14:-1}*/-1
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: /*${15:'path'}*/'path'
          }
        ]
      }
    }
    options: {
      throughput: /*${16:'throughput'}*/'throughput'
    }
  }
}
