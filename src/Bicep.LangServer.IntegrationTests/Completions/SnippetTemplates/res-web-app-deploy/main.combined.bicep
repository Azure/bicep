resource webApplication 'Microsoft.Web/sites@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
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

