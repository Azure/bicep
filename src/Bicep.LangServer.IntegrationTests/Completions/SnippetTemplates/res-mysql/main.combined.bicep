// $1 = mySQLdb
// $2 = 'name'
// $3 = 'administratorLogin'
// $4 = 'administratorLoginPassword'
// $5 = 'Default'

resource mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
    createMode: 'Default'
  }
}
// Insert snippet here

