// $1 = 'sqlDatabase/import'
// $2 = location
// $3 = sqlDatabaseImport
// $4 = StorageAccessKey
// $5 = 'storageKey'
// $6 = 'storageUri'
// $7 = 'administratorLogin'
// $8 = 'administratorLoginPassword'

param location string

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: 'sqlDatabase/import'
  location: location
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

