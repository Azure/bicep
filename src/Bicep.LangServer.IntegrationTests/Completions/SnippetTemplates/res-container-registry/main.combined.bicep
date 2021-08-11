// $1 = containerRegistry
// $2 = 'name'
// $3 = 'Classic'
// $4 = true

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'Classic'
  }
  properties: {
    adminUserEnabled: true
  }
}
// Insert snippet here

