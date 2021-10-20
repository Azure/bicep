// $1 = appServicePlan
// $2 = 'name'

param location string

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
}
// Insert snippet here

