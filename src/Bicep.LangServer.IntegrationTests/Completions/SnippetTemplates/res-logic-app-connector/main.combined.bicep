// $1 = logicAppConnector
// $2 = 'name'
// $3 = 'name'
// $4 = 'logicAppConnectorApi'

param location string

resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'name'
  location: location
  properties: {
    name: 'name'
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', location, 'logicAppConnectorApi')
    })
  }
}
// Insert snippet here

