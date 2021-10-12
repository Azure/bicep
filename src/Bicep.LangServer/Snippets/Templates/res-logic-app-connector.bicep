// Logic App Connector
resource /*${1:logicAppConnector}*/logicAppConnector 'Microsoft.Web/connections@2016-06-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, /*${3:'logicAppConnectorApi'}*/'logicAppConnectorApi')
    })
  }
}
