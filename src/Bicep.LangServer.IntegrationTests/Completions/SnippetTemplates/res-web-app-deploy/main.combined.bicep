// $1 = 'name'
// $2 = location
// $3 = webApplicationExtension
// $4 = 'packageUri'
// $5 = 'connectionString'
// $6 = 'name'

param location string

resource webApplication 'Microsoft.Web/sites@2020-12-01' = {
  name: 'name'
  location: location
}

resource webApplicationExtension 'Microsoft.Web/sites/extensions@2020-12-01' = {
  parent: webApplication
  name: 'MSDeploy'
  properties: {
    packageUri: 'packageUri'
    dbType: 'None'
    connectionString: 'connectionString'
    setParameters: {
      'IIS Web Application Name': 'name'
    }
  }
}
// Insert snippet here

