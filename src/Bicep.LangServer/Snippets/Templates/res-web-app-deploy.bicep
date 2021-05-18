// Web Deploy for a Web Application
resource webApplication 'Microsoft.Web/sites@2020-12-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
}

resource ${2:'webApplicationExtension} 'Microsoft.Web/sites/extensions@2020-12-01' = {
  parent: webApplication
  name: 'MSDeploy'
  properties: {
    packageUri: ${3:'packageUri'}
    dbType: 'None'
    connectionString: ${4:'connectionString'}
    setParameters: {
      'IIS Web Application Name': ${5:'name'}
    }
  }
}
