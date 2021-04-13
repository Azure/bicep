// MySQL Database
resource MySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: ${1:MySQLdb}
  location: resourceGroup().location
  properties: {
    administratorLogin: ${2:adminUsername}
    administratorLoginPassword: ${3:adminPassword}
    createMode: ${4|Default,GeoRestore,PointInTimeRestore,Replica|}
  }
}