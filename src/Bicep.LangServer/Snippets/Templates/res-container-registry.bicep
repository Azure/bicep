// Container Registry
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
  name: '${1:containerRegistry}'
  location: resourceGroup().location
  sku: {
    name: '${2|Classic,Basic,Standard,Premium|}'
  }
  properties: {
    adminUserEnabled: '${3|true,false|}'
  }
}