// Logic App Connector
resource ${1:'logicAppConnector'} 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    name: ${2:'logicAppConnector'}
    apiDefinitionUrl: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, ${3:'logicAppConnectorApi'})
  }
}
