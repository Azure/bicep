// $1 = webServerFarms
// $2 = 'name'
// $3 = webApplication
// $4 = 'name'
// $5 = appServicePlan

resource webServerFarms 'Microsoft.Web/serverfarms@2021-01-15' existing = {
  name: 'name'
} 

resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name'
  location: resourceGroup().location
  tags: {
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan': 'Resource'
  }
  properties: {
    serverFarmId: webServerFarms.id
  }
}// Insert snippet here
