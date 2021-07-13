// SQL Database Import
resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:'location'}*/'location'
}

resource /*${3:sqlDatabaseImport}*/sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: 'import'
  properties: {
    storageKeyType: '${4|StorageAccessKey,SharedAccessKey|}'
    storageKey: ${5:'storageKey'}
    storageUri: ${6:'storageUri'}
    administratorLogin: ${7:'administratorLogin'}
    administratorLoginPassword: ${8:'administratorLoginPassword'}
    operationMode: 'Import'
  }
}
