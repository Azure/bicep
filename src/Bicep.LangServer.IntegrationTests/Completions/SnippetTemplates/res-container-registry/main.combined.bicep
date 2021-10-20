// $1 = containerRegistry
// $2 = 'name'
// $3 = 'Basic'
// $4 = false

param location string

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: 'name'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}
// Insert snippet here

