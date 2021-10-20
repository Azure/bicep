// Container Registry
resource /*${1:containerRegistry}*/containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  sku: {
    name: /*${3|'Basic','Standard','Premium'|}*/'Basic'
  }
  properties: {
    adminUserEnabled: /*${4|true,false|}*/false
  }
}
