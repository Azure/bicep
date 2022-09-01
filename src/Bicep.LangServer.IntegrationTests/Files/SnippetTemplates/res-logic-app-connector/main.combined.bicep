// $1 = logicAppConnector
// $2 = 'name'
// $3 = location
// $4 = 'logicAppConnectorApi'

param location string

resource logicAppConnector 'Microsoft.Web/connections@2016-06-01' = {
  name: 'name'
  location: location
  properties: {
    api: any({
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', location, 'logicAppConnectorApi')
    })
  }
}
// Insert snippet here

