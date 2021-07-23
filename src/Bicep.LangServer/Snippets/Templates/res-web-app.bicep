// Web Application
resource /*${1:webServerFarms}*/webServerFarms 'Microsoft.Web/serverfarms@2021-01-15' existing = {
  name: /*${2:'name'}*/'name'
} 

resource /*${3:webApplication}*/webApplication 'Microsoft.Web/sites@2018-11-01' = {
  name: /*${4:'name'}*/'name'
  location: resourceGroup().location
  tags: {
    /*'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${5:appServicePlan}'*/resource: 'Resource'
  }
  properties: {
    serverFarmId: webServerFarms.id
  }
}