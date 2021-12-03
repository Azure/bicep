// Web Deploy for a Web Application
resource webApplication 'Microsoft.Web/sites@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: location
}

resource /*${2:webApplicationExtension}*/webApplicationExtension 'Microsoft.Web/sites/extensions@2020-12-01' = {
  parent: webApplication
  name: 'MSDeploy'
  properties: {
    packageUri: /*${3:'packageUri'}*/'packageUri'
    dbType: 'None'
    connectionString: /*${4:'connectionString'}*/'connectionString'
    setParameters: {
      'IIS Web Application Name': /*${5:'name'}*/'name'
    }
  }
}
