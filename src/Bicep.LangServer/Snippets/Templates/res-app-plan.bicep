// Application Service Plan (Server Farm)
resource /*${1:appServicePlan}*/appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
}
