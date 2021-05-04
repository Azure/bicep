resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    name: 'name'
    apiDefinitionUrl: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'logicAppConnectorApi')
  }
}

