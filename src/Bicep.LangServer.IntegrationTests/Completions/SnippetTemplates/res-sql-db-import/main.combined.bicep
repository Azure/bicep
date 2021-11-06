// $1 = 'sqlDatabase'
// $2 = 'import'
// $3 = 'location'
// $4 = sqlDatabaseImport
// $5 = StorageAccessKey
// $6 = 'storageKey'
// $7 = 'storageUri'
// $8 = 'administratorLogin'
// $9 = 'administratorLoginPassword'

resource sqlServer 'Microsoft.Sql/servers@2014-04-01' existing = {
  name: 'sqlDatabase'
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: 'import'
  location: 'location'
}

resource sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: 'import'
  properties: {
    storageKeyType: 'StorageAccessKey'
    storageKey: 'storageKey'
    storageUri: 'storageUri'
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
    operationMode: 'Import'
  }
}
// Insert snippet here

