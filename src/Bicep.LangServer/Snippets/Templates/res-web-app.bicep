// Web Application
resource ${1:webApplication} 'Microsoft.Web/sites@2018-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  tags: {
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${3:'appServicePlan'}': 'Resource'
  }
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', ${4:'appServicePlan'})
  }
}