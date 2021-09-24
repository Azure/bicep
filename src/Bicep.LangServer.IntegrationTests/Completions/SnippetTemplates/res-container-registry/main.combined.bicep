// $1 = containerRegistry
// $2 = 'name'
// $3 = 'Classic'
// $4 = true

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}
// Insert snippet here

