param sqlAdministratorLogin string

@secure()
param sqlAdministratorLoginPassword string

@allowed([
  'Enabled'
  'Disabled'
])
param transparentDataEncryption string = 'Enabled'

param location string = resourceGroup().location

var databaseName = 'sample-db-with-tde'
var databaseEdition = 'Basic'
var databaseCollation = 'SQL_Latin1_General_CP1_CI_AS'
var databaseServiceObjectiveName = 'Basic'

var sqlServerName = 'sqlserver${uniqueString(resourceGroup().id)}'

resource sqlServer 'Microsoft.Sql/servers@2020-02-02-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: '12.0'
  }
}

resource db 'Microsoft.Sql/servers/databases@2020-02-02-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: databaseServiceObjectiveName
    tier: databaseEdition
  }
  properties: {
    collation: databaseCollation
  }
}

// very long type...
resource tde 'Microsoft.Sql/servers/databases/transparentDataEncryption@2020-08-01-preview' = {
  parent: db
  name: 'current'
  properties: {
    state: transparentDataEncryption
  }
}
