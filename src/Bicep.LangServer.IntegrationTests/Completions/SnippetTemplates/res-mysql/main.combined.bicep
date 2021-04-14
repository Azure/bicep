resource MySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'testMySQLdb'
  location: resourceGroup().location
  properties: {
    administratorLogin: 'testUsername'
    administratorLoginPassword: 'testPassword'
    createMode: 'Default'
  }
}
