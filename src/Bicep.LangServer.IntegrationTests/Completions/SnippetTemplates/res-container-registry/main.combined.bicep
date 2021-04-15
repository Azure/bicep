resource containerRegistry 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
  name: 'testContainerRegistry'
  location: resourceGroup().location
  sku: {
    name: 'Classic'
  }
  properties: {
    adminUserEnabled: true
  }
}
