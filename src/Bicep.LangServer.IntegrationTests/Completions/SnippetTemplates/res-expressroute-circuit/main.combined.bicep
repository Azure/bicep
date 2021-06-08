resource expressRouteCircuit 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    family: 'MeteredData'
    tier: 'Local'
    name: 'Local_MeteredData'
  }
  properties: {
    serviceProviderProperties: {
      bandwidthInMbps: 50
      peeringLocation: 'Amsterdam'
      serviceProviderName: 'Telia Carrier'
    }
    allowClassicOperations: false
  }
}
