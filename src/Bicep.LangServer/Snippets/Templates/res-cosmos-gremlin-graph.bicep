// Cosmos DB Gremlin Graph
resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: /*${1:'name'}*/'account-name/gremlin/database-name'
  properties: {
    resource: {
      id: /*${2:'id'}*/'id'
    }
    options: {
      throughput: /*${3:'throughput'}*/'throughput'
    }
  }
}

resource /*${4:cosmosDbGremlinGraph}*/cosmosDbGremlinGraph 'Microsoft.DocumentDb/databaseAccounts/apis/databases/graphs@2016-03-31' = {
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
        indexingMode: /*'${9|Consistent,Lazy,None|}'*/'Consistent'
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
