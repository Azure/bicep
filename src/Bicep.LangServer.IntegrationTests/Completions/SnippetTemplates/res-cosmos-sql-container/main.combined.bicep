// $1 = 'accountName/sql/databaseName'
// $2 = 'id'
// $3 = sqlContainerName
// $4 = 'name'
// $5 = 'id'
// $6 = 'paths'
// $7 = Hash
// $8 = Consistent
// $9 = 'path'
// $10 = Hash
// $11 = String
// $12 = -1
// $13 = 'path'

resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: 'accountName/sql/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}

resource sqlContainerName 'Microsoft.DocumentDb/databaseAccounts/apis/databases/containers@2016-03-31' = {
  parent: sqlDb 
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
        indexingMode: 'Consistent'
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
    options: {}
  }
}
// Insert snippet here

