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

