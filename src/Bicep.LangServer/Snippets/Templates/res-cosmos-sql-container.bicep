// Cosmos DB SQL Container
resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: ${1:'name'}
  properties: {
    resource: {
      id: ${2:'id'}
    }
    options: {}
  }
}

resource ${3:sqlContainerName} 'Microsoft.DocumentDb/databaseAccounts/apis/databases/containers@2016-03-31' = {
  parent: sqlDb 
  name: ${4:'name'}
  properties: {
    resource: {
      id: ${5:'id'}
      partitionKey: {
        paths: [
          ${6:'paths'}
        ]
        kind: '${7|Hash,Range|}'
      }
      indexingPolicy: {
        indexingMode: '${8|Consistent,Lazy,None|}'
        includedPaths: [
          {
            path: ${9:'path'}
            indexes: [
              {
                kind: '${10|Hash,Range,Spatial|}'
                dataType: '${11|String,Number,Point,Polygon,LineString,MultiPolygon|}'
                precision: ${12:'precision'}
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: ${13:'path'}
          }
        ]
      }
    }
    options: {}
  }
}
