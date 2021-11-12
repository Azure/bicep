// Cosmos DB Gremlin Graph
resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases@2021-06-15' = {
  name: /*${1:'name'}*/'account-name/database-name'
  properties: {
    resource: {
      id: /*${2:'id'}*/'id'
    }
    options: {
      throughput: /*${3:'throughput'}*/'throughput'
    }
  }
}

resource /*${4:cosmosDbGremlinGraph}*/cosmosDbGremlinGraph 'Microsoft.DocumentDB/databaseAccounts/gremlinDatabases/graphs@2021-06-15' = {
  parent: gremlinDb
  name: /*${5:'name'}*/'name'
  properties: {
    resource: {
      id: /*${6:'id'}*/'id'
      partitionKey: {
        paths: [
          /*${7:'paths'}*/'paths'
        ]
        kind: /*'${8|Hash,Range|}'*/'Hash'
      }
      indexingPolicy: {
        indexingMode: /*'${9|consistent,lazy,none|}'*/'consistent'
        includedPaths: [
          {
            path: /*${10:'path'}*/'path'
            indexes: [
              {
                kind: /*'${11|Hash,Range,Spatial|}'*/'Hash'
                dataType: /*'${12|String,Number,Point,Polygon,LineString,MultiPolygon|}'*/'String'
                precision: /*${13:-1}*/-1
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: /*${14:'path'}*/'path'
          }
        ]
      }
    }
    options: {
      throughput: /*${15:'throughput'}*/'throughput'
    }
  }
}
