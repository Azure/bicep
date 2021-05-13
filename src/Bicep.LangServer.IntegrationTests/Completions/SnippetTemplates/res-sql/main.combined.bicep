resource sqlServer 'Microsoft.Sql/servers@2020-11-01-preview' ={
  name: 'name'
  location: resourceGroup().location
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: 'name'
  location: resourceGroup().location
  properties: {
    collation: 'collation'
    edition: 'Basic'
    maxSizeBytes: 'maxSizeBytes'
    requestedServiceObjectiveName: 'Basic'
  }
}

