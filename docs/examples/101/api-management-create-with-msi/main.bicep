param publisherEmail string {
  minLength: 1
}
param publisherName string {
  minLength: 1
}
param sku string {
  default: 'Developer'
  allowed: [
    'Consumption'
    'Developer'
    'Basic'
    'Standard'
    'Premium'
  ]
}
param skuCount int {
  default: 1
}
param location string = resourceGroup().location

resource apiManagement 'Microsoft.ApiManagement/service@2020-06-01-preview' = {
  name: 'apiservice${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: sku
    capacity: skuCount
  }
  properties: {
    publisherName: publisherName
    publisherEmail: publisherEmail
  }
  identity: {
    type: 'SystemAssigned'
  }
}
