// Service Bus Namespace
resource /*${1:serviceBusNamespace}*/serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  sku: {
    name: /*${3|'Basic','Standard','Premium'|}*/'Standard'
    capacity: /*${4:1}*/1
    tier: /*${5|'Basic','Standard','Premium'|}*/'Standard'
  }
}
