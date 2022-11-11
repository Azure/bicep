// Container Registry
resource /*${1:containerRegistry}*/containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  sku: {
    name: /*${4|'Basic','Standard','Premium'|}*/'Basic'
  }
  properties: {
    adminUserEnabled: /*${5|true,false|}*/false
  }
}
