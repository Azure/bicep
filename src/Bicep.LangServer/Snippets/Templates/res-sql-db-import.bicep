// SQL Database Import
resource sqlServer 'Microsoft.Sql/servers@2014-04-01' existing = {
  name: /*${1:'name'}*/'sqlDatabase'
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: /*${2:'name'}*/'Import'
  location: /*${3:'location'}*/'location'
}

resource /*${4:sqlDatabaseImport}*/sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: 'import'
  properties: {
    storageKeyType: /*'${5|StorageAccessKey,SharedAccessKey|}'*/'StorageAccessKey'
    storageKey: /*${6:'storageKey'}*/'storageKey'
    storageUri: /*${7:'storageUri'}*/'storageUri'
    administratorLogin: /*${8:'administratorLogin'}*/'administratorLogin'
    administratorLoginPassword: /*${9:'administratorLoginPassword'}*/'administratorLoginPassword'
    operationMode: 'Import'
  }
}
