// $1 = 'name'
// $2 = location
// $3 = sqlServerDatabase
// $4 = 'name'
// $5 = 'collation'
// $6 = Basic
// $7 = 'maxSizeBytes'
// $8 = Basic

param location string

resource sqlServer 'Microsoft.Sql/servers@2014-04-01' ={
  name: 'name'
  location: location
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: 'name'
  location: location
  properties: {
    collation: 'collation'
    edition: 'Basic'
    maxSizeBytes: 'maxSizeBytes'
    requestedServiceObjectiveName: 'Basic'
  }
}
// Insert snippet here

