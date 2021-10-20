// $1 = expressRouteCircuit
// $2 = 'name'
// $3 = 'MeteredData'
// $4 = 'Local'
// $5 = 'Local_MeteredData'
// $6 = 50
// $7 = 'Amsterdam'
// $8 = 'Telia Carrier'
// $9 = false

param location string

resource expressRouteCircuit 'Microsoft.Network/expressRouteCircuits@2020-11-01' = {
  name: 'name'
  location: location
  sku:{
    family: 'MeteredData'
    tier: 'Local'
    name: 'Local_MeteredData'
  }
  properties:{
    serviceProviderProperties:{
      bandwidthInMbps: 50
        peeringLocation: 'Amsterdam'
      serviceProviderName: 'Telia Carrier'
    }
    allowClassicOperations:false
  }
}
// Insert snippet here

