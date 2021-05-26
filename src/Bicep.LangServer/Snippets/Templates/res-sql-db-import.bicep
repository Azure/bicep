// SQL Database Import
resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: ${1:'name'}
  location: ${2:'location'}
}

resource ${3:sqlDatabaseImport} 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: ${4:'name'}
  properties: {
    storageKeyType: '${5|StorageAccessKey,SharedAccessKey|}'
    storageKey: ${6:'storageKey'}
    storageUri: ${7:'storageUri'}
    administratorLogin: ${8:'administratorLogin'}
    administratorLoginPassword: ${9:'administratorLoginPassword'}
    operationMode: ${10:'operationMode'}
  }
}
