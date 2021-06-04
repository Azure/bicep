// $1 = webApplication
// $2 = 'name'
// $3 = appServicePlan
// $4 = 'appServicePlan'

resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name'
  location: resourceGroup().location
  tags: {
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan': 'Resource'
  }
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', 'appServicePlan')
  }
}// Insert snippet here

