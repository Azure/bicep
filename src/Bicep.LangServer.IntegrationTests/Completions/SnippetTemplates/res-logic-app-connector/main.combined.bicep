resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'testLogicAppConnector'
  location: resourceGroup().location
  properties: {
    name: 'testLogicAppConnector'
    apiDefinitionUrl: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'testLogicAppConnectorApi')
  }
}
