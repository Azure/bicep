@minLength(1)
param publisherEmail string

@minLength(1)
param publisherName string

@allowed([
  'Consumption'
  'Developer'
  'Basic'
  'Standard'
  'Premium'
])
param sku string = 'Developer'

param skuCount int = 1
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
