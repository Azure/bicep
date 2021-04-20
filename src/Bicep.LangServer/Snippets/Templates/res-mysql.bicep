// MySQL Database
resource ${1:'mySQLdb'} 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    administratorLogin: ${3:'administratorLogin'}
    administratorLoginPassword: ${4:'administratorLoginPassword'}
    createMode: ${5|'Default','GeoRestore','PointInTimeRestore','Replica'|}
  }
}
