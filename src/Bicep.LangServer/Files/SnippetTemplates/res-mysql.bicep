// MySQL Database
resource /*${1:mySQLdb}*/mySQLdb 'Microsoft.DBforMySQL/servers@2017-12-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    administratorLogin: /*${4:'administratorLogin'}*/'administratorLogin'
    administratorLoginPassword: /*${5:'administratorLoginPassword'}*/'administratorLoginPassword'
    createMode: /*${6|'Default','GeoRestore','PointInTimeRestore','Replica'|}*/'Default'
  }
}
