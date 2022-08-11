// $1 = location

@secure()
param location string// Insert snippet here

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

