// $1 = 'name'
// $2 = sqlServerDatabase
// $3 = 'name'
// $4 = 'collation'
// $5 = Basic
// $6 = 'maxSizeBytes'
// $7 = Basic

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

