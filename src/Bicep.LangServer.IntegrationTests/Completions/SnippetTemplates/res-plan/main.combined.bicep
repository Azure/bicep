resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'appServicePlan'
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

