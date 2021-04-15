resource containerRegistry 'Microsoft.ContainerRegistry/registries@2020-11-01-preview' = {
  name: 'testContainerRegistry'
  location: resourceGroup().location
  sku: {
    name: 'Classic'
  }
  properties: {
    adminUserEnabled: true
  }
}
