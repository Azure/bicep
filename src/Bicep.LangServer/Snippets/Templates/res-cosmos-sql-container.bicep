// Cosmos DB SQL Container
resource ${1:sqlContainerName} 'Microsoft.DocumentDb/databaseAccounts/apis/databases/containers@2016-03-31' = {
  parent: databaseAccount 
  name: ${2:'name'}
  properties: {
    resource: {
      id: ${3:'id'}
      partitionKey: {
        paths: [
          ${4:'paths'}
        ]
        kind: '${5|Hash,Range|}'
      }
      indexingPolicy: {
        indexingMode: '${6|Consistent,Lazy,None|}'
        includedPaths: [
          {
            path: ${7:'path'}
            indexes: [
              {
                kind: '${8|Hash,Range,Spatial|}'
                dataType: '${9|String,Number,Point,Polygon,LineString,MultiPolygon|}'
                precision: ${10:'precision'}
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: ${11:'path'}
          }
        ]
      }
    }
    options: {}
  }
}

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: ${12:'name'}
  properties: {
    resource: {
      id: ${13:'id'}
    }
    options: {}
  }
}
