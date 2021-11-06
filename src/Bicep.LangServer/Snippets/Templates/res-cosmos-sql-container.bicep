// Cosmos DB SQL Container
resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: /*${1:'name'}*/'account-name'
}

resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: /*${2:'name'}*/'database-name'
  properties: {
    resource: {
      id: /*${3:'id'}*/'id'
    }
    options: {}
  }
}

resource /*${4:sqlContainerName}*/sqlContainerName 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  parent: sqlDb 
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
                precision: /*${13:'precision'}*/'precision'
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
    options: {}
  }
}
