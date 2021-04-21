// Logic App Connector
resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: ${1:'logicAppConnector'}
  location: resourceGroup().location
  properties: {
    name: ${2:'logicAppConnector'}
    apiDefinitionUrl: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, ${3:'logicAppConnectorApi'})
  }
}