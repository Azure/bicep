// Logic App Connector
resource /*${1:logicAppConnector}*/logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    name: /*${4:'name'}*/'name'
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', /*${3:location}*/'location', /*${5:'logicAppConnectorApi'}*/'logicAppConnectorApi')
    })
  }
}
