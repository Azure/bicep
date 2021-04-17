// MySQL Database
resource mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: ${1:'mySQLdb'}
  location: resourceGroup().location
  properties: {
    administratorLogin: ${2:'administratorLogin'}
    administratorLoginPassword: ${3:'administratorLoginPassword'}
    createMode: '${4|Default,GeoRestore,PointInTimeRestore,Replica|}'
  }
}
