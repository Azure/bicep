param sqlAdministratorLogin string
param sqlAdministratorLoginPassword string {
  secure: true
}

param transparentDataEncryption string {
  default: 'Enabled'
  allowed: [
    'Enabled'
    'Disabled'
  ]
}

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
  name: '${sqlServer.name}/${databaseName}' // originally using sqlServerName param, but dependsOn was not automatically added
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
resource tde 'Microsoft.Sql/servers/databases/transparentDataEncryption@2017-03-01-preview' = {
  name: '${db.name}/current' // had to change databaseName => db.name to get dependsOn working
  properties: {
    status: transparentDataEncryption
  }
}