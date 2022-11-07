// Web Application
resource /*${1:webApplication}*/webApplication 'Microsoft.Web/sites@2021-01-15' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  tags: {
    /*'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${4:appServicePlan}'*/resource: 'Resource'
  }
  properties: {
    serverFarmId: /*${5:'webServerFarms.id'}*/'webServerFarms.id'
  }
}
