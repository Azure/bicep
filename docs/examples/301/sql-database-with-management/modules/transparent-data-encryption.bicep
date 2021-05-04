param sqlDatabase object
param sqlServerName string

resource transparentDataEncryption 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: '${sqlServerName}/${sqlDatabase.name}/current'
  properties: {
    status: sqlDatabase.dataEncryption
  }
}
