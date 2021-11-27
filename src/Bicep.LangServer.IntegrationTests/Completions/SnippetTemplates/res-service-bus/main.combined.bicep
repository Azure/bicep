// $1 = serviceBusNamespace
// $2 = 'name'
// $3 = 'Basic'
// $4 = 1
// $5 = 'Basic'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'Basic'
    capacity: 1
    tier: 'Basic'
  }
}
// Insert snippet here

