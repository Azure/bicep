resource mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'mySQLdb'
  location: resourceGroup().location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
    createMode: 'Default'
  }
}

