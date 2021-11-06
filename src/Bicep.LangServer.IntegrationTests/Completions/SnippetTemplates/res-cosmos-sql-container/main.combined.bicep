// $1 = 'accountName'
// $2 = 'databaseName'
// $3 = 'id'
// $4 = sqlContainerName
// $5 = 'name'
// $6 = 'id'
// $7 = 'paths'
// $8 = Hash
// $9 = consistent
// $10 = 'path'
// $11 = Hash
// $12 = String
// $13 = -1
// $14 = 'path'

resource cosmosDBAccount 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: 'accountName'
}

resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  parent: cosmosDBAccount
  name: 'databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {}
  }
}

resource sqlContainerName 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
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
    options: {}
  }
}
// Insert snippet here

