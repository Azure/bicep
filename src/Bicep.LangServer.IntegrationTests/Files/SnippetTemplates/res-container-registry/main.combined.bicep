// $1 = containerRegistry
// $2 = 'registryName'
// $3 = location
// $4 = 'Basic'
// $5 = false

param location string

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: 'registryName'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}


