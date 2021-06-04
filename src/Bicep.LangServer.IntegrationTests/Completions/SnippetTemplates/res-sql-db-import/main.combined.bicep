// $1 = 'sqlDatabase/import'
// $2 = 'location'
// $3 = sqlDatabaseImport
// $4 = 'name'
// $5 = StorageAccessKey
// $6 = 'storageKey'
// $7 = 'storageUri'
// $8 = 'administratorLogin'
// $9 = 'administratorLoginPassword'
// $10 = 'operationMode'

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: 'sqlDatabase/import'
  location: 'location'
}

resource sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: 'name'
  properties: {
    storageKeyType: 'StorageAccessKey'
    storageKey: 'storageKey'
    storageUri: 'storageUri'
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
    operationMode: 'operationMode'
  }
}
// Insert snippet here

