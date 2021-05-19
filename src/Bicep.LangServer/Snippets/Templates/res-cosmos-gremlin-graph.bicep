// Cosmos DB Gremlin Graph
resource gremlinDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: ${1:'name'}
  properties: {
    resource: {
      id: ${2:'id'}
    }
    options: {
      throughput: ${3:'throughput'}
    }
  }
}

resource ${4:cosmosDbGremlinGraph} 'Microsoft.DocumentDb/databaseAccounts/apis/databases/graphs@2016-03-31' = {
  parent: gremlinDb
  name: ${5:'name'}
  properties: {
    resource: {
      id: ${6:'id'}
      partitionKey: {
        paths: [
          ${7:'paths'}
        ]
        kind: '${8|Hash,Range|}'
      }
      indexingPolicy: {
        indexingMode: '${9|Consistent,Lazy,None|}'
        includedPaths: [
          {
            path: ${10:'path'}
            indexes: [
              {
                kind: '${11|Hash,Range,Spatial|}'
                dataType: '${12|String,Number,Point,Polygon,LineString,MultiPolygon|}'
                precision: ${13:-1}
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: ${14:'path'}
          }
        ]
      }
    }
    options: {
      throughput: ${15:'throughput'}
    }
  }
}
