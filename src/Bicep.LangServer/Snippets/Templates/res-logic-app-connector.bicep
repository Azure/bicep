// Logic App Connector
resource /*${1:logicAppConnector}*/logicAppConnector 'Microsoft.Web/connections@2016-06-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', /*${3:location}*/'location', /*${4:'logicAppConnectorApi'}*/'logicAppConnectorApi')
    })
  }
}
