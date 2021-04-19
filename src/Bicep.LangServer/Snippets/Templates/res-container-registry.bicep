// Container Registry
resource ${1:'containerRegistry'} 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: '${2|Classic,Basic,Standard,Premium|}'
  }
  properties: {
    adminUserEnabled: ${3|true,false|}
  }
}
