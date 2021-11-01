// Cosmos DB SQL Container
resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  name: /*${1:'name'}*/'account-name/database-name'
  properties: {
    resource: {
      id: /*${2:'id'}*/'id'
    }
    options: {}
  }
}

resource /*${3:sqlContainerName}*/sqlContainerName 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  parent: sqlDb 
  name: /*${4:'name'}*/'name'
  properties: {
    resource: {
      id: /*${5:'id'}*/'id'
      partitionKey: {
        paths: [
          /*${6:'paths'}*/'paths'
        ]
        kind: /*'${7|Hash,Range|}'*/'Hash'
      }
      indexingPolicy: {
        indexingMode: /*'${8|consistent,lazy,none|}'*/'consistent'
        includedPaths: [
          {
            path: /*${9:'path'}*/'path'
            indexes: [
              {
                kind: /*'${10|Hash,Range,Spatial|}'*/'Hash'
                dataType: /*'${11|String,Number,Point,Polygon,LineString,MultiPolygon|}'*/'String'
                precision: /*${12:'precision'}*/'precision'
              }
            ]
          }
        ]
        excludedPaths: [
          {
            path: /*${13:'path'}*/'path'
          }
        ]
      }
    }
    options: {}
  }
}
