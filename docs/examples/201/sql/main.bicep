parameter sqlAdministratorLogin string
parameter sqlAdministratorLoginPassword string {
  secure: true
}

parameter transparentDataEncryption string {
  defaultValue: 'Enabled'
  allowedValues: [
    'Enabled'
    'Disabled'
  ]
}

parameter location string = resourceGroup().location

variable databaseName = 'sample-db-with-tde'
variable databaseEdition = 'Basic'
variable databaseCollation = 'SQL_Latin1_General_CP1_CI_AS'
variable databaseServiceObjectiveName = 'Basic'

variable sqlServerName = 'sqlserver${uniqueString(subscription().id, resourceGroup().id)}'
resource sqlServer 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    
  }
}