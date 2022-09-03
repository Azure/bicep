// $1 = serviceBusNamespace
// $2 = 'name'
// $3 = location
// $4 = 'Basic'
// $5 = 1
// $6 = 'Basic'

param location string

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: 'name'
  location: location
  sku: {
    name: 'Basic'
    capacity: 1
    tier: 'Basic'
  }
}
// Insert snippet here

