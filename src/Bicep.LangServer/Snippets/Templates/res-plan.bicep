// Application Service Plan (Server Farm)
resource appServicePlan 'Microsoft.Web/serverfarms@2018-02-01' = {
  name: ${1:appServicePlan}
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}