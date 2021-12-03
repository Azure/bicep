// MySQL Database
resource /*${1:mySQLdb}*/mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    administratorLogin: /*${3:'administratorLogin'}*/'administratorLogin'
    administratorLoginPassword: /*${4:'administratorLoginPassword'}*/'administratorLoginPassword'
    createMode: /*${5|'Default','GeoRestore','PointInTimeRestore','Replica'|}*/'Default'
  }
}
