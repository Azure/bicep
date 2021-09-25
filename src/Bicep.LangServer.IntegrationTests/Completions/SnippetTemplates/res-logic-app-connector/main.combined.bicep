// $1 = logicAppConnector
// $2 = 'name'
// $3 = 'name'
// $4 = 'logicAppConnectorApi'

resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    name: 'name'
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'logicAppConnectorApi')
    })
  }
}
// Insert snippet here

