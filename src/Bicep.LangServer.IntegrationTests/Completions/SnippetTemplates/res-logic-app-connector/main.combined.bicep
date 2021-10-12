// $1 = logicAppConnector
// $2 = 'name'
// $3 = 'logicAppConnectorApi'

resource logicAppConnector 'Microsoft.Web/connections@2016-06-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'logicAppConnectorApi')
    })
  }
}
// Insert snippet here

