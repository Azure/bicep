// $1 = logicAppConnector
// $2 = 'name'
// $3 = 'name'
// $4 = 'logicAppConnectorApi'

resource logicAppConnector 'Microsoft.Web/connections@2015-08-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    name: 'name'
    api: {
//@[4:7) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "location". (CodeDescription: none) |api|
      id: subscriptionResourceId('Microsoft.Web/locations/managedApis', resourceGroup().location, 'logicAppConnectorApi')
    }
  }
}
// Insert snippet here

