// $1 = sqlDb
// $2 = 'accountName/sql/databaseName'
// $3 = 'id'
// $4 = 'throughput'

resource sqlDb 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: 'accountName/sql/databaseName'
  properties: {
    resource: {
      id: 'id'
    }
    options: {
      throughput: 'throughput'
    }
  }
}
// Insert snippet here

