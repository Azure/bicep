// Service Bus Namespace
resource /*${1:serviceBusNamespace}*/serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  sku: {
    name: /*${4|'Basic','Standard','Premium'|}*/'Standard'
    capacity: /*${5:1}*/1
    tier: /*${6|'Basic','Standard','Premium'|}*/'Standard'
  }
}
