// $1 = mySQLdb
// $2 = 'name'
// $3 = location
// $4 = 'administratorLogin'
// $5 = 'administratorLoginPassword'
// $6 = 'Default'

param location string

resource mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'name'
  location: location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
    createMode: 'Default'
  }
}
// Insert snippet here

