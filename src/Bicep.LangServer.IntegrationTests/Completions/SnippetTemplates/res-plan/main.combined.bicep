// $1 = appServicePlan
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 1

param location string

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: location
  sku: {
    name: 'name'
    capacity: 1
  }
}
// Insert snippet here

