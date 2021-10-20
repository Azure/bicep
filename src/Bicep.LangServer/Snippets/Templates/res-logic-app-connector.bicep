// Logic App Connector
resource /*${1:logicAppConnector}*/logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    name: /*${3:'name'}*/'name'
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', location, /*${4:'logicAppConnectorApi'}*/'logicAppConnectorApi')
    })
  }
}
