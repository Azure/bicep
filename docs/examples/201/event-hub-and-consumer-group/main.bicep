param namespaceName string
param eventHubSku string {
  default: 'Standard'
  allowed: [
    'Standard'
    'Basic'
  ]
}
param skuCapacity int {
  default: 1
  allowed: [
    1
    2
    4
  ]
}
param eventHubName string
param consumerGroupName string
param location string = resourceGroup().location

resource namespace 'Microsoft.EventHub/namespaces@2017-04-01' = {
  name: namespaceName
  location: location
  sku: {
    name: eventHubSku
    tier: eventHubSku
    capacity: skuCapacity
  }
  properties: {}
}

resource eventHub 'Microsoft.EventHub/namespaces/eventhubs@2017-04-01' = {
  name: '${namespace.name}/${eventHubName}'
  properties: {}
}

resource consumerGroup 'Microsoft.EventHub/namespaces/eventhubs/consumergroups@2017-04-01' = {
  name: '${eventHub.name}/${consumerGroupName}'
  properties: {}
}
