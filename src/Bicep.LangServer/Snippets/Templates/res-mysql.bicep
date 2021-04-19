// MySQL Database
resource ${1:'mySQLdb'} 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    administratorLogin: ${2:'administratorLogin'}
    administratorLoginPassword: ${3:'administratorLoginPassword'}
    createMode: '${4|Default,GeoRestore,PointInTimeRestore,Replica|}'
  }
}
