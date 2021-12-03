// SQL Database Import
resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: /*${1:'name'}*/ 'sqlDatabase/Import'
}

resource /*${3:sqlDatabaseImport}*/ sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: 'import'
  properties: {
    storageKeyType: /*'${4|StorageAccessKey,SharedAccessKey|}'*/ 'StorageAccessKey'
    storageKey: /*${5:'storageKey'}*/ 'storageKey'
    storageUri: /*${6:'storageUri'}*/ 'storageUri'
    administratorLogin: /*${7:'administratorLogin'}*/ 'administratorLogin'
    administratorLoginPassword: /*${8:'administratorLoginPassword'}*/ 'administratorLoginPassword'
    operationMode: 'Import'
  }
}
