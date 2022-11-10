// Web Deploy for a Web Application
resource webApplication 'Microsoft.Web/sites@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
}

resource /*${3:webApplicationExtension}*/webApplicationExtension 'Microsoft.Web/sites/extensions@2020-12-01' = {
  parent: webApplication
  name: 'MSDeploy'
  properties: {
    packageUri: /*${4:'packageUri'}*/'packageUri'
    dbType: 'None'
    connectionString: /*${5:'connectionString'}*/'connectionString'
    setParameters: {
      'IIS Web Application Name': /*${6:'name'}*/'name'
    }
  }
}
