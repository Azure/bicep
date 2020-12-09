param circuitName string
param serviceProviderName string
param peeringLocation string
param bandwidthInMbps int
param skuTier string {
  default: 'Standard'
  allowed: [
    'Standard'
    'Premium'
  ]
}
param skuFamily string {
  default: 'MeteredData'
  allowed: [
    'MeteredData'
    'UnlimitedData'
  ]
}
param location string = resourceGroup().location

resource circuit 'Microsoft.Network/expressRouteCircuits@2020-06-01' = {
  name: circuitName
  location: location
  sku: {
    name: '${skuTier}_${skuFamily}'
    tier: skuTier
    family: skuFamily
  }
  properties: {
    serviceProviderProperties: {
      serviceProviderName: serviceProviderName
      peeringLocation: peeringLocation
      bandwidthInMbps: bandwidthInMbps
    }
  }
}
