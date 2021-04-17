resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'logicAppConnector'
  location: resourceGroup().location
  properties: {
    name: 'logicAppConnector'
    apiDefinitionUrl: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'logicAppConnectorApi')
  }
}
