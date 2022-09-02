// $1 = webApplication
// $2 = 'name'
// $3 = location
// $4 = appServicePlan
// $5 = 'webServerFarms.id'

param location string

resource webApplication 'Microsoft.Web/sites@2021-01-15' = {
  name: 'name'
  location: location
  tags: {
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan': 'Resource'
  }
  properties: {
    serverFarmId: 'webServerFarms.id'
  }
}
// Insert snippet here

