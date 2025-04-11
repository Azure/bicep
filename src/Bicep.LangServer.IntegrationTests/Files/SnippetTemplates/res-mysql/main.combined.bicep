// $1 = mySQLdb
// $2 = 'name'
// $3 = location
// $4 = 'administratorLogin'
// $5 = password
// $6 = 'Default'

param location string

@secure()
param password string

resource mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'name'
  location: location
  properties: {
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: password
    createMode: 'Default'
  }
}


