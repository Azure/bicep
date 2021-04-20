// Application Service Plan (Server Farm)
resource ${1:'appServicePlan'} 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}
